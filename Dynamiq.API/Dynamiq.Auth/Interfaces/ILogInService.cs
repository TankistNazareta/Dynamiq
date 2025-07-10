using Dynamiq.Auth.DTOs;

namespace Dynamiq.Auth.Interfaces
{
    public interface ILogInService
    {
        Task<AuthResponseDto> LogIn(AuthUserDto authUser);
    }
}
