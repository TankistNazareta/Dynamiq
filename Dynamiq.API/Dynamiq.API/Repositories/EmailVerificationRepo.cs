using AutoMapper;
using Dynamiq.API.DAL.Context;
using Dynamiq.API.DAL.Models;
using Dynamiq.API.Interfaces;
using Dynamiq.API.Mapping.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Dynamiq.API.Repositories
{
    public class EmailVerificationRepo : IEmailVerificationRepo
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public EmailVerificationRepo(IMapper mapper, AppDbContext db)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task Delete(Guid id)
        {
            var model = await _db.EmailVerifications.FirstOrDefaultAsync(x => x.Id == id);

            if (model == null)
                throw new KeyNotFoundException($"{id} does not exist");

            _db.EmailVerifications.Remove(model);

            await _db.SaveChangesAsync();
        }

        public async Task<List<EmailVerificationDto>> GetAll()
        {
            var models = await _db.EmailVerifications.ToListAsync();

            return _mapper.Map<List<EmailVerificationDto>>(models);
        }

        public async Task<EmailVerificationDto> GetById(Guid id)
        {
            var model = await _db.EmailVerifications
                .Include(ev => ev.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (model == null)
                throw new KeyNotFoundException($"Email verification with the id: {id} does not exist");

            return _mapper.Map<EmailVerificationDto>(model);
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

        public async Task<EmailVerificationDto> Insert(EmailVerificationDto entity)
        {
            var model = _mapper.Map<EmailVerification>(entity);

            _db.EmailVerifications.Add(model);

            await _db.SaveChangesAsync();

            return _mapper.Map<EmailVerificationDto>(model);
        }

        public async Task ConfirmEmail(Guid id)
        {
            var model = await _db.EmailVerifications.FirstOrDefaultAsync(x => x.Id == id);

            if (model == null)
                throw new KeyNotFoundException($"token with id: {id} does not exist, or he was expired");
            if (model.ExpiresAt < DateTime.UtcNow)
                throw new TimeoutException("Token expired");
            if (model.ConfirmedEmail)
                throw new InvalidOperationException("This token has already been activated.");

            model.ConfirmedEmail = true;
            await _db.SaveChangesAsync();
        }

        public async Task<EmailVerificationDto> Update(EmailVerificationDto entity)
        {
            var newModel = _mapper.Map<EmailVerification>(entity);

            _db.EmailVerifications.Update(newModel);

            await _db.SaveChangesAsync();

            return _mapper.Map<EmailVerificationDto>(newModel);
        }
    }
}
