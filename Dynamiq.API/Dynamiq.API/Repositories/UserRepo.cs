using AutoMapper;
using Dynamiq.API.DAL.Context;
using Dynamiq.API.DAL.Models;
using Dynamiq.API.Interfaces;
using Dynamiq.API.Mapping.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Dynamiq.API.Repositories
{
    public class UserRepo : DefaultCrudRepo<UserDto, User>, IUserRepo
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public UserRepo(IMapper mapper, AppDbContext db) : base(db, mapper)
        {
            _mapper = mapper;
            _db = db;
        }

        protected virtual IQueryable<User> Query()
        {
            return _db.Users
                .Include(u => u.PaymentHistories)
                .Include(u => u.Subscriptions)
                .Include(u => u.EmailVerification);
        }


        public async Task<UserDto> GetByEmail(string email)
        {
            var model = await _db.Users
                .Include(u => u.PaymentHistories)
                .Include(u => u.Subscriptions)
                .Include(u => u.EmailVerification)
                .FirstOrDefaultAsync(x => x.Email == email);

            if (model == null)
                throw new KeyNotFoundException($"User with the email: {email} wasn't found");

            return _mapper.Map<UserDto>(model);
        }
    }
}
