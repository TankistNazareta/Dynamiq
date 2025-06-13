using Dynamiq.API.Extension.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamiq.API.Extension.DTOs
{
    public class UserDto
    {
        public Guid? Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public RoleEnum Role { get; set; }
        public bool ConfirmedEmail { get; set; }
    }
}
