using AutoMapper;
using Dynamiq.API.DAL.Context;
using Dynamiq.API.DAL.Models;
using Dynamiq.API.Interfaces;
using Dynamiq.API.Mapping.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Dynamiq.API.Repositories
{
    public class EmailVerificationRepo : DefaultCrudRepo<EmailVerificationDto, EmailVerification>, IEmailVerificationRepo
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public EmailVerificationRepo(IMapper mapper, AppDbContext db) : base(db, mapper) 
        {
            _db = db;
            _mapper = mapper;
        }

        protected virtual IQueryable<EmailVerification> Query()
        {
            return _db.EmailVerifications.Include(ev => ev.User);
        }

        public async Task<EmailVerificationDto> GetByToken(string token)
        {
            var model = await _db.EmailVerifications
                    .Include(ev => ev.User)
                    .FirstOrDefaultAsync(x => x.Token == token);

            if (model == null)
                throw new KeyNotFoundException($"Email verification with the token: {token} does not exist");

            return _mapper.Map<EmailVerificationDto>(model);
        }
    }
}
