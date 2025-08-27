using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Domain.Entities;
using Dynamiq.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dynamiq.Infrastructure.Repositories
{
    public class CartItemRepo : ICartItemRepo
    {
        private readonly AppDbContext _db;

        public CartItemRepo(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(CartItem cartItem, CancellationToken ct)
            => await _db.CartItems.AddAsync(cartItem, ct);

        public async Task<List<CartItem>> GetByCartIdAsync(Guid cartId, CancellationToken ct)
            => await _db.CartItems
                    .Where(ci => ci.CartId == cartId)
                    .ToListAsync(ct);

        public void Remove(CartItem cartItem)
            => _db.CartItems.Remove(cartItem);

        public void Update(CartItem cartItem)
            => _db.CartItems.Update(cartItem);
    }
}
