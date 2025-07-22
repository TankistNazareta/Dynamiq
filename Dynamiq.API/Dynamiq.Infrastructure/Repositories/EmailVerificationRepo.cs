using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Interfaces.Repositories;
using Dynamiq.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dynamiq.Infrastructure.Repositories
{
    public class EmailVerificationRepo : IEmailVerificationRepo
    {
        private readonly AppDbContext _db;

        public EmailVerificationRepo(AppDbContext db)
        {
            _db = db;
        }

        public async Task<EmailVerification?> GetByTokenAsync(string token, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be empty.", nameof(token));

            return await _db.EmailVerifications.FirstOrDefaultAsync(x => x.Token == token, ct);
        }

        public async Task AddAsync(EmailVerification emailVerification, CancellationToken ct)
            => await _db.EmailVerifications.AddAsync(emailVerification, ct);
    }
}
