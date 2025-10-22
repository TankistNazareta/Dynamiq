using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Domain.Aggregates;
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

        public async Task AddAsync(Subscription subscription, CancellationToken cancellationToken)
            => await _db.Subscriptions.AddAsync(subscription, cancellationToken);

        public void Delete(Subscription subscription)
            => _db.Subscriptions.Remove(subscription);

        public async Task<List<Subscription>> GetAllAsync(CancellationToken cancellationToken)
            => await _db.Subscriptions.ToListAsync(cancellationToken);

        public Task<Subscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => _db.Subscriptions.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
}
