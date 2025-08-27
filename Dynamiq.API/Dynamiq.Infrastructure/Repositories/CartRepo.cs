using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Domain.Aggregates;
using Dynamiq.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dynamiq.Infrastructure.Repositories
{
    public class CartRepo : ICartRepo
    {
        private readonly AppDbContext _db;

        public CartRepo(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Cart cart, CancellationToken ct)
                => await _db.Carts.AddAsync(cart, ct);

        public async Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken ct)
                => await _db.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);

        public void Remove(Cart cart)
                => _db.Carts.Remove(cart);
    }
}
