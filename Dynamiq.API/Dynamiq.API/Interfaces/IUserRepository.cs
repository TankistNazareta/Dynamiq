using Dynamiq.API.Extension.DTOs;

namespace Dynamiq.API.Interfaces
{
    public interface IUserRepository : ICRUD<UserDto>
    {
        public Task<UserDto> GetByEmail(string email);
    }
}
