using Dynamiq.Application.DTOs;
using Dynamiq.Domain.Enums;

namespace Dynamiq.Application.Interfaces.Auth
{
    public interface ITokenService
    {
        AuthResponseDto CreateAuthResponse(string email, RoleEnum role);
    }
}
