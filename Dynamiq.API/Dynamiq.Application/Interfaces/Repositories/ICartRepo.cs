using Dynamiq.Domain.Aggregates;

namespace Dynamiq.Application.Interfaces.Repositories
{
    public interface ICartRepo
    {
        Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken ct);
        Task AddAsync(Cart cart, CancellationToken ct);
        void Remove(Cart cart);
    }
}
