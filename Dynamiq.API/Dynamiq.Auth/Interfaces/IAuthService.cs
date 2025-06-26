using Dynamiq.Auth.DTOs;

namespace Dynamiq.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LogIn(AuthUserDto authUser);
        Task<AuthResponseDto> SignUp(AuthUserDto authUser);
        Task<AuthResponseDto> Refresh(string token);
        Task Revoke(string token);
    }
}
