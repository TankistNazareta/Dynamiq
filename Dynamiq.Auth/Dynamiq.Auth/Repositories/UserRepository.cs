using AutoMapper;
using Dynamiq.Auth.DAL.Context;
using Dynamiq.Auth.DAL.Models;
using Dynamiq.Auth.DTOs;
using Dynamiq.Auth.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

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

        public async Task CheckPassword(UserDto user)
        {
            var model = await _db.Users.FirstOrDefaultAsync(x => x.Email == user.Email);

            if (model == null)
                throw new ArgumentException("Your email isn't correct");

            if (!CheckPassword(user.Password, model.PasswordHash))
                throw new ArgumentException("Your password is incorrect");

            //Sign in method 
        }

        public async Task Delete(Guid id)
        {
            var model = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (model == null)
                throw new ArgumentException($"{nameof(id)} does not exist");

            _db.Users.Remove(model);

            await _db.SaveChangesAsync();
        }

        public async Task<List<UserDto>> GetAll()
        {
            var models = await _db.Users.ToListAsync();

            return _mapper.Map<List<UserDto>>(models);
        }

        public async Task<UserDto> GetByEmail(string email)
        {
            var model = await _db.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (model == null)
                throw new ArgumentException($"user with the email: {nameof(email)} does not exist");

            return _mapper.Map<UserDto>(model);
        }

        public async Task<UserDto> GetById(Guid id)
        {
            var model = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (model == null)
                throw new ArgumentException($"user with the id: {nameof(id)} does not exist");

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
            var newDataUser = await _db.Users.FindAsync(user.Id);
            if (newDataUser == null)
                throw new ArgumentException("User was not found");

            newDataUser.Email = user.Email;
            newDataUser.Role = user.Role;
            newDataUser.ConfirmedEmail = user.ConfirmedEmail;
            newDataUser.PasswordHash = HashPassword(user.Password);
            newDataUser.Role = user.Role;

            _db.Users.Update(newDataUser);

            await _db.SaveChangesAsync();

            return _mapper.Map<UserDto>(newDataUser);
        }

        private string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
        private bool CheckPassword(string password, string HashedPassword) 
            => BCrypt.Net.BCrypt.Verify(password, HashedPassword);
    }
}
