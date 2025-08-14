using Dynamiq.Application.DTOs.AuthDTOs;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Dynamiq.Application.Interfaces.Auth
{
    public interface IGoogleOidcService
    {
        string GetLoginGoogleUrlHandler(string? returnUrl = null);
        string BuildAuthorizationUrl(string state, string nonce);
        Task<GoogleTokenResponse> ExchangeCodeAsync(string code, CancellationToken ct);
        Task<(ClaimsPrincipal Principal, SecurityToken Token)> ValidateIdTokenAsync(string idToken, CancellationToken ct);
    }
}
