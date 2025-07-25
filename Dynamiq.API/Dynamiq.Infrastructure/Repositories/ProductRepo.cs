using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Interfaces.Repositories;
using Dynamiq.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dynamiq.Infrastructure.Repositories
{
    public class ProductRepo : IProductRepo
    {
        private readonly AppDbContext _db;

        public ProductRepo(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Product product, CancellationToken ct)
            => await _db.Products.AddAsync(product, ct);

        public void Delete(Product product)
                => _db.Products.Remove(product);

        public Task DeleteAsync(Product product, CancellationToken ct)
        {
            _db.Products.Remove(product);

            return Task.CompletedTask;
        }

        public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct)
                => await _db.Products.AsNoTracking().ToListAsync(ct);

        public async Task<IReadOnlyList<Product>> GetAllBySlugAsync(string slug, CancellationToken ct)
        {
            var category = await _db.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Slug == slug, ct);

            return category?.Products.ToList() ?? new List<Product>();
        }

        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct)
                => await _db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task<IReadOnlyList<Product>> GetOnlySubscriptions(CancellationToken ct)
                => await _db.Products
                            .Where(p => p.Interval != Domain.Enums.IntervalEnum.OneTime)
                            .ToListAsync(ct);
    }
}
