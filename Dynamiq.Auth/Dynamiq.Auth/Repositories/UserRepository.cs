using AutoMapper;
using Dynamiq.Auth.DAL.Context;
using Dynamiq.Auth.DAL.Models;
using Dynamiq.Auth.DTOs;
using Dynamiq.Auth.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dynamiq.Auth.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMapper _mapper;
        private readonly AuthDbContext _db;

        public UserRepository(IMapper mapper, AuthDbContext db)
        {
            _mapper = mapper;
            _db = db;
        }

        public async Task<bool> CheckPassword(UserDto user)
        {
            var model = await _db.Users.FirstOrDefaultAsync(x => x.Email == user.Email);

            if(model.PasswordHash == HashPassword(user.Password))
                return true;
            return false;
        }

        public async Task<bool> Delete(Guid id)
        {
            var model = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (model == null)
                return false;

            _db.Users.Remove(model);

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<List<UserDto>> GetAll()
        {
            var models = await _db.Users.ToListAsync();

            return _mapper.Map<List<UserDto>>(models);
        }

        public async Task<UserDto> GetByEmail(string email)
        {
            var model = await _db.Users.FirstOrDefaultAsync(x => x.Email == email);

            return _mapper.Map<UserDto>(model);
        }

        public async Task<UserDto> GetById(Guid id)
        {
            var model = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<UserDto>(model);
        }

        public async Task<UserDto> Insert(UserDto user)
        {
            if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password))
                throw new ArgumentNullException(nameof(user));

            var model = new User()
            {
                Email = user.Email,
                Id = new Guid(),
                Role = user.Role,
                ConfirmedEmail = false,
                PasswordHash = HashPassword(user.Password)
            };

            _db.Users.Add(model);

            await _db.SaveChangesAsync();

            return _mapper.Map<UserDto>(model);
        }

        public async Task<UserDto> Update(UserDto user)
        {
            var oldUser = await _db.Users.FirstOrDefaultAsync(x => x.Email == user.Email);

            if (oldUser == null)
                throw new ArgumentException(nameof(user));

            var updatedUser = new User()
            {
                Email = oldUser.Email,
                Id = oldUser.Id,
                PasswordHash = HashPassword(user.Password),
                Role = user.Role,
                ConfirmedEmail = user.ConfirmedEmail,
            };

            _db.Users.Update(updatedUser);

            await _db.SaveChangesAsync();

            return _mapper.Map<UserDto>(updatedUser);
        }

        private string HashPassword(string password)
        {
            return password.GetHashCode().ToString();
        }
    }
}
