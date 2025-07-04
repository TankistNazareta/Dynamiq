using Dynamiq.API.Extension.Interfaces;
using Dynamiq.API.Mapping.DTOs;

namespace Dynamiq.API.Interfaces
{
    public interface IUserRepo : ICRUD<UserDto>
    {
        public Task<UserDto> GetByEmail(string email);
    }
}
