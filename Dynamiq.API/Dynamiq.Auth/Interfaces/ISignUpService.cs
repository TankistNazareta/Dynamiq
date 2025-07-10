using Dynamiq.Auth.DTOs;

namespace Dynamiq.Auth.Interfaces
{
    public interface ISignUpService
    {
        Task SignUp(AuthUserDto authUser);
    }
}
