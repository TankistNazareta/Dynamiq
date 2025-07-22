using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Interfaces.Repositories;
using Dynamiq.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dynamiq.Infrastructure.Repositories
{
    public class SubscriptionRepo : ISubscriptionRepo
    {
        private readonly AppDbContext _db;

        public SubscriptionRepo(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Subscription subscription, CancellationToken ct)
                    => await _db.Subscriptions.AddAsync(subscription, ct);

        public async Task<IReadOnlyList<Subscription>> GetAllByUserIdAsync(Guid userId, CancellationToken ct)
                    => await _db.Subscriptions.Where(s => s.UserId == userId).ToListAsync();
        public async Task<Subscription?> GetByIdAsync(Guid id, CancellationToken ct)
                    => await _db.Subscriptions.FirstOrDefaultAsync(s => s.Id == id, ct);
    }
}
