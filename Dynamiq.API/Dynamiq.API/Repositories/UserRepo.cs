using AutoMapper;
using Dynamiq.API.DAL.Context;
using Dynamiq.API.DAL.Models;
using Dynamiq.API.Interfaces;
using Dynamiq.API.Mapping.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Dynamiq.API.Repositories
{
    public class UserRepo : IUserRepo
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public UserRepo(IMapper mapper, AppDbContext db)
        {
            _mapper = mapper;
            _db = db;
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
            var models = await _db.Users
                .Include(u => u.PaymentHistories)
                .Include(u => u.Subscriptions)
                .Include(u => u.EmailVerification)
                .ToListAsync();

            return _mapper.Map<List<UserDto>>(models);
        }

        public async Task<UserDto> GetByEmail(string email)
        {
            var model = await _db.Users
                .Include(u => u.PaymentHistories)
                .Include(u => u.Subscriptions)
                .Include(u => u.EmailVerification)
                .FirstOrDefaultAsync(x => x.Email == email);

            if (model == null)
                throw new ArgumentException($"user with the email: {nameof(email)} does not exist");

            return _mapper.Map<UserDto>(model);
        }

        public async Task<UserDto> GetById(Guid id)
        {
            var model = await _db.Users
                .Include(u => u.PaymentHistories)
                .Include(u => u.Subscriptions)
                .Include(u => u.EmailVerification)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (model == null)
                throw new ArgumentException($"user with the id: {nameof(id)} does not exist");

            return _mapper.Map<UserDto>(model);
        }

        public async Task<UserDto> Insert(UserDto user)
        {
            if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.PasswordHash))
                throw new ArgumentNullException(nameof(user));

            var model = _mapper.Map<User>(user);

            _db.Users.Add(model);

            await _db.SaveChangesAsync();

            return _mapper.Map<UserDto>(model);
        }

        public async Task<UserDto> Update(UserDto user)
        {
            var newDataUser = _mapper.Map<User>(user);

            _db.Users.Update(newDataUser);

            await _db.SaveChangesAsync();

            return _mapper.Map<UserDto>(newDataUser);
        }

        public async Task<int> RemoveAllExpiredUsers()
        {
            var expiredUsers = await _db.Users
                .Include(u => u.EmailVerification)
                .Where(u =>
                u.EmailVerification.ExpiresAt < DateTime.UtcNow &&
                !u.EmailVerification.ConfirmedEmail)
                .ToListAsync();

            if (expiredUsers.Count == 0)
                return 0;

            _db.Users.RemoveRange(expiredUsers);
            await _db.SaveChangesAsync();

            return expiredUsers.Count;
        }
    }
}
