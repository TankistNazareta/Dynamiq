using Dynamiq.Domain.Entities;

namespace Dynamiq.Domain.Interfaces.Repositories
{
    public interface IUserRepo
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken ct);
        Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
        Task AddAsync(User user, CancellationToken ct);
        void Delete(User user);
        Task<List<User>> GetAllExpiredUsersAsync(CancellationToken ct);
        Task<User?> GetUserWithEmailVerificationByEmail(string email, CancellationToken ct);
    }
}