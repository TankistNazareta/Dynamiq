using Dynamiq.Auth.DTOs;

namespace Dynamiq.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<string> Login(AuthUserDto authUser);
        Task<string> Signup(AuthUserDto authUser);
    }
}
