using Dynamiq.Auth.DAL.Models;
using Dynamiq.Auth.DTOs;

namespace Dynamiq.Auth.Interfaces
{
    public interface IUserRepository
    {
        public Task<UserDto> Update(UserDto user);
        public Task<List<UserDto>> GetAll();
        public Task<UserDto> GetById(Guid id);
        public Task Delete(Guid id);
        public Task<UserDto> Insert(UserDto user);
        public Task<UserDto> GetByEmail(string email);
        public Task CheckPassword(UserDto user);
    }
}
