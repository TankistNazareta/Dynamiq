using Dynamiq.Application.DTOs.AuthDTOs;
using Dynamiq.Application.Interfaces.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Dynamiq.Infrastructure.Services.Auth
{
    public class GoogleOidcService : IGoogleOidcService
    {
        private readonly GoogleOAuthOptions _opts;
        private readonly HttpClient _http;
        private readonly IConfigurationManager<OpenIdConnectConfiguration> _configManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const string Authority = "https://accounts.google.com";

        public GoogleOidcService(
            System.Net.Http.IHttpClientFactory httpFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            var request = httpContextAccessor.HttpContext?.Request;

            _opts = new GoogleOAuthOptions() 
            {
                ClientId = Environment.GetEnvironmentVariable("GOOGLE_OAUTH_CLIENT_ID") 
                    ?? throw new ArgumentNullException("GoogleOAuth:ClientId is not configured"),
                ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_OAUTH_CLIENT_SECRET")
                    ?? throw new ArgumentNullException("GoogleOAuth:ClientSecret is not configured"),
                RedirectUri = Environment.GetEnvironmentVariable("GOOGLE_OAUTH_REDIRECT")
                    ?? throw new ArgumentNullException("GOOGLEOAUTH:REDIRECT is not configured")
            };
            
            _http = httpFactory.CreateClient("google-oauth");
            _httpContextAccessor = httpContextAccessor;

            var documentRetriever = new HttpDocumentRetriever { RequireHttps = true };
            _configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                "https://accounts.google.com/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever(),
                documentRetriever
            );
        }


        public string BuildAuthorizationUrl(string state, string nonce)
        {
            // Response type = code (Authorization Code flow)
            // Scope must include openid email profile to receive email in id_token
            var url = new StringBuilder();
            url.Append("https://accounts.google.com/o/oauth2/v2/auth?");
            url.Append($"client_id={Uri.EscapeDataString(_opts.ClientId)}");
            url.Append($"&redirect_uri={Uri.EscapeDataString(_opts.RedirectUri)}");
            url.Append("&response_type=code");
            url.Append("&scope=" + Uri.EscapeDataString("openid email profile"));
            url.Append("&access_type=offline"); // may return a Google refresh_token on first consent
            url.Append("&include_granted_scopes=true");
            url.Append($"&state={Uri.EscapeDataString(state)}");
            url.Append($"&nonce={Uri.EscapeDataString(nonce)}");
            return url.ToString();
        }

        public async Task<GoogleTokenResponse> ExchangeCodeAsync(string code, CancellationToken ct)
        {
            // Exchange authorization code for tokens
            using var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = _opts.ClientId,
                ["client_secret"] = _opts.ClientSecret,
                ["redirect_uri"] = _opts.RedirectUri,
                ["grant_type"] = "authorization_code"
            });

            using var resp = await _http.PostAsync("https://oauth2.googleapis.com/token", content, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);
            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"Google token endpoint error: {resp.StatusCode} {body}");

            var tokens = JsonSerializer.Deserialize<GoogleTokenResponse>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ??
                throw new InvalidOperationException("Failed to parse Google token response");

            return tokens;
        }

        public async Task<(ClaimsPrincipal Principal, SecurityToken Token)> ValidateIdTokenAsync(string idToken, CancellationToken ct)
        {
            var config = await _configManager.GetConfigurationAsync(ct);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(idToken);

            bool hasExp = jwt.Payload.Exp.HasValue;

            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = Authority,
                ValidateIssuer = true,
                ValidAudience = _opts.ClientId,
                ValidateAudience = true,
                IssuerSigningKeys = config.SigningKeys,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = hasExp,
                ClockSkew = TimeSpan.FromMinutes(2)
            };

            var principal = handler.ValidateToken(idToken, validationParameters, out var validatedToken);
            return (principal, validatedToken);
        }


        public string GetLoginGoogleUrlHandler(string? returnUrl = null)
        {
            var state = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
            var nonce = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));

            var response = _httpContextAccessor.HttpContext!.Response;

            response.Cookies.Append("g_state", state, new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Lax });
            response.Cookies.Append("g_nonce", nonce, new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Lax });
            if (!string.IsNullOrWhiteSpace(returnUrl))
                response.Cookies.Append("g_return", returnUrl, new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Lax });

            return BuildAuthorizationUrl(state, nonce);
        }
    }
}
