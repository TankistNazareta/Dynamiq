using Dynamiq.Domain.Entities;

namespace Dynamiq.Domain.Interfaces.Repositories
{
    public interface IEmailVerificationRepo
    {
        Task AddAsync(EmailVerification emailVerification, CancellationToken ct);
        Task<EmailVerification> GetByTokenAsync(string token, CancellationToken ct);
    }
}
