using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Interfaces.Repositories;
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
                                    .Include(u => u.Subscriptions)
                                    .Include(u => u.PaymentHistories)
                                    .Include(u => u.RefreshToken)
                                    .FirstOrDefaultAsync(u => u.Email == email, ct);

        public async Task<User?> GetByEmailVerificationTokenAsyc(string token, CancellationToken ct)
                    => await _db.Users
                                    .Include(u => u.EmailVerification)
                                    .Include(u => u.Subscriptions)
                                    .Include(u => u.PaymentHistories)
                                    .Include(u => u.RefreshToken)
                                    .FirstOrDefaultAsync(u => u.EmailVerification.Token == token);

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
                    => await _db.Users
                                    .Include(u => u.EmailVerification)
                                    .Include(u => u.Subscriptions)
                                    .Include(u => u.PaymentHistories)
                                    .Include(u => u.RefreshToken)
                                    .FirstOrDefaultAsync(u => u.Id == id, ct);

        public async Task<User?> GetByRefreshTokenAsync(string token, CancellationToken ct)
                    => await _db.Users
                                    .Include(u => u.EmailVerification)
                                    .Include(u => u.Subscriptions)
                                    .Include(u => u.PaymentHistories)
                                    .Include(u => u.RefreshToken)
                                    .FirstOrDefaultAsync(u => u.RefreshToken.Token == token);
    }
}
