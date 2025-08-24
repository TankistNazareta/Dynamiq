using Dynamiq.Domain.Aggregates;

namespace Dynamiq.Application.Interfaces.Repositories
{
    public interface IUserRepo
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken ct);
        Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
        Task AddAsync(User user, CancellationToken ct);
        void Delete(User user);
        Task<List<User>> GetAllExpiredUsersAsync(CancellationToken ct);
        Task<User?> GetByEmailVerificationTokenAsync(string token, CancellationToken ct);
        Task<User?> GetByRefreshTokenAsync(string token, CancellationToken ct);
    }
}