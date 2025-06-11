using Dynamiq.Auth.DAL.Enums;

namespace Dynamiq.Auth.DTOs
{
    public class UserDto
    {
        public Guid? Id { get; set; }
        public string Email { get; set; }
        public RoleEnum Role { get; set; } = RoleEnum.User;
        public bool ConfirmedEmail { get; set; } = false;
        public string? Password { get; set; }
    }
}
