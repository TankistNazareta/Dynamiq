using Dynamiq.Domain.Aggregates;

namespace Dynamiq.Application.Interfaces.Repositories
{
    public interface ISubscriptionRepo
    {
        public Task AddAsync(Subscription subscription, CancellationToken cancellationToken);
        public void Delete(Subscription subscription);
        public Task<Subscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        public Task<List<Subscription>> GetAllAsync(CancellationToken cancellationToken);
    }
}
