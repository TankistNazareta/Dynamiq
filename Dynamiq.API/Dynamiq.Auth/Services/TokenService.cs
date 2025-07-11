using Dynamiq.API.Extension.Enums;
using Dynamiq.API.Mapping.DTOs;
using Dynamiq.Auth.DTOs;
using Dynamiq.Auth.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text;

namespace Dynamiq.Auth.Services
{
    public class TokenService : ITokenService
    {
        private readonly HttpClient _apiClient;

        private readonly ILogger _logger;

        private readonly string _jwtKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;

        public TokenService(IConfiguration config, IHttpClientFactory apiClient, ILogger<TokenService> logger)
        {
            _apiClient = apiClient.CreateClient("ApiClient");

            _logger = logger;

            _jwtKey = config["JwtSettings:Key"]!;
            _jwtIssuer = config["JwtSettings:Issuer"]!;
            _jwtAudience = config["JwtSettings:Audience"]!;
        }

        public async Task<AuthResponseDto> Refresh(string token)
        {
            // Get token
            var responseToken = await _apiClient.GetFromJsonAsync<RefreshTokenDto>($"/refresh/{token}")
                ?? throw new Exception("Failed to refresh token: token not found");

            //Check token
            if (!responseToken.IsRevoked)
                throw new ArgumentException("Your token is revoked");

            if (responseToken.ExpiresAt < DateTime.UtcNow)
                throw new ArgumentException("Your token has expired");

            // Revoke old token
            var responsePut = await _apiClient.PutAsync($"/refresh/{token}", null);
            if (!responsePut.IsSuccessStatusCode)
                throw new Exception("Failed to revoke token");

            var authResponseDto = await CreateAuthResponse(new()
            {
                Id = responseToken.User.Id,
                Email = responseToken.User.Email,
                Role = responseToken.User.Role,
            }, responseToken.ExpiresAt);

            _logger.LogInformation("{FirstToken} token refreshed to {RefreshedToken}", token, authResponseDto.RefreshToken);

            return authResponseDto;
        }

        public async Task Revoke(string token)
        {
            var response = await _apiClient.PutAsync($"/refresh/{token}", null);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to revoke new refresh token");

            _logger.LogInformation("{Token} token was revoked", token);
        }

        public async Task<AuthResponseDto> CreateAuthResponse(UserDto user, DateTime? expiresAt = null)
        {
            var accessToken = GenerateJwtToken(user.Email, user.Role);
            var refreshToken = GenerateRefreshToken(user.Id, expiresAt);

            var refreshTokenJson = new StringContent(
                JsonSerializer.Serialize(refreshToken),
                Encoding.UTF8,
                "application/json");

            var response = await _apiClient.PostAsync("/refresh", refreshTokenJson);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to Log in: {response.Content}");

            return new()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
        }

        private string GenerateJwtToken(string email, RoleEnum role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private RefreshTokenDto GenerateRefreshToken(Guid userId, DateTime? expiresAt)
        {
            var bytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);

            var refreshTokenString = Convert.ToBase64String(bytes);

            return new()
            {
                Token = refreshTokenString,
                UserId = userId,
                ExpiresAt = expiresAt ?? DateTime.UtcNow.AddDays(7)
            };
        }
    }
}
