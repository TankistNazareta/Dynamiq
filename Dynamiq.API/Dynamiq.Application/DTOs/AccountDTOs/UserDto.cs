using Dynamiq.Domain.Enums;

namespace Dynamiq.Application.DTOs.AccountDTOs
{
    public record UserDto(Guid Id, string Email, RoleEnum Role);
}
