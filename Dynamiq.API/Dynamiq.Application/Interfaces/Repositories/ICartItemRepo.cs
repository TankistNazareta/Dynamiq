using Dynamiq.Domain.Entities;

namespace Dynamiq.Application.Interfaces.Repositories
{
    public interface ICartItemRepo
    {
        Task AddAsync(CartItem cartItem, CancellationToken ct);
        void Update(CartItem cartItem);
        void Remove(CartItem cartItem);
        Task<List<CartItem>> GetByCartIdAsync(Guid cartId, CancellationToken ct);
    }
}
