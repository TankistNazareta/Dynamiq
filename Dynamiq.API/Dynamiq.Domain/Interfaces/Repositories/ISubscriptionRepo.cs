using Dynamiq.Domain.Entities;

namespace Dynamiq.Domain.Interfaces.Repositories
{
    public interface ISubscriptionRepo
    {
        Task<Subscription?> GetByIdAsync(Guid id, CancellationToken ct);
        Task AddAsync(Subscription subscription, CancellationToken ct);
        Task<IReadOnlyList<Subscription>> GetAllByUserIdAsync(Guid userId, CancellationToken ct);
    }
}
