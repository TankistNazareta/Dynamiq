using Dynamiq.Domain.Enums;
using Dynamiq.Domain.Enums;

namespace Dynamiq.Application.DTOs
{
    public record UserDto(Guid Id, string Email, RoleEnum Role);
}
