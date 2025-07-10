using Dynamiq.API.Mapping.DTOs;
using Dynamiq.Auth.DTOs;

namespace Dynamiq.Auth.Interfaces
{
    public interface ITokenService
    {
        Task<AuthResponseDto> CreateAuthResponse(UserDto user, DateTime? expiresAt = null);
        Task Revoke(string token);
        Task<AuthResponseDto> Refresh(string token);
    }
}
