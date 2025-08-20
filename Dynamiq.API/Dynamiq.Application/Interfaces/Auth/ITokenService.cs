using Dynamiq.Application.DTOs.AuthDTOs;
using Dynamiq.Domain.Enums;

namespace Dynamiq.Application.Interfaces.Auth
{
    public interface ITokenService
    {
        AuthTokensDto CreateAuthResponse(string email, RoleEnum role, Guid userId);
    }
}
