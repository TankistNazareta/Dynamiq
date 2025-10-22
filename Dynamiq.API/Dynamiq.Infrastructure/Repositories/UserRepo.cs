using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Domain.Aggregates;
using Dynamiq.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dynamiq.Infrastructure.Repositories
{
    public class UserRepo : IUserRepo
    {
        private readonly AppDbContext _db;

        public UserRepo(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(User user, CancellationToken ct)
                    => await _db.Users.AddAsync(user, ct);

        public void Delete(User user)
                    => _db.Users.Remove(user);

        public async Task<List<User>> GetAllExpiredUsersAsync(CancellationToken ct)
                    => await _db.Users
                                .Include(u => u.EmailVerification)
                                .Where(u => !u.EmailVerification.IsConfirmed && u.EmailVerification.ExpiresAt <= DateTime.UtcNow)
                                .ToListAsync();

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
                    => await _db.Users
                        .Include(u => u.EmailVerification)
                        .Include(u => u.PaymentHistories)
                            .ThenInclude(ph => ph.Subscription)
                        .Include(u => u.RefreshTokens)
                        .FirstOrDefaultAsync(u => u.Email == email, ct);

        public async Task<User?> GetByEmailVerificationTokenAsync(string token, CancellationToken ct)
                    => await _db.Users
                               .Include(u => u.EmailVerification)
                               .Include(u => u.RefreshTokens)
                               .FirstOrDefaultAsync(u => u.EmailVerification.Token == token);

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
                    => await _db.Users
                                    .Include(u => u.EmailVerification)
                                    .Include(u => u.PaymentHistories)
                                        .ThenInclude(ph => ph.Subscription)
                                    .Include(u => u.RefreshTokens)
                                    .FirstOrDefaultAsync(u => u.Id == id, ct);

        public async Task<User?> GetByRefreshTokenAsync(string token, CancellationToken ct)
                    => await _db.Users
                                    .Include(u => u.RefreshTokens)
                                    .Include(u => u.EmailVerification)
                                    .FirstOrDefaultAsync(u =>
                                                    u.RefreshTokens.Any(rt => rt.Token == token &&
                                                    !rt.IsRevoked &&
                                                    rt.ExpiresAt > DateTime.UtcNow), ct);

        public Task<Guid?> GetUserIdByHisEmailAsync(string email, CancellationToken ct)
                    => _db.Users
                            .Where(u => u.Email == email)
                            .Select(u => (Guid?)u.Id)
                            .FirstOrDefaultAsync(ct);
    }
}
