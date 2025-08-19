using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Common;
using Dynamiq.Domain.Interfaces.Repositories;
using Dynamiq.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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

        public async Task<IReadOnlyList<Product>> GetAllAsync(int limit, int offset, CancellationToken ct)
        {
            var query = _db.Products.AsNoTracking();

            int skip = offset;
            int take = limit;

            return await query
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);
        }


        public async Task<IReadOnlyList<Product>> GetAllBySlugAsync(string slug, int limit, int offset, CancellationToken ct)
        {
            var category = await _db.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Slug == slug, ct);

            var products = category?.Products.AsQueryable() ?? Enumerable.Empty<Product>().AsQueryable();

            int skip = offset;
            int take = limit;

            return await products
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct)
                => await _db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task<List<Product>> GetFilteredAsync(ProductFilter request, int limit, int offset, CancellationToken ct)
        {
            var query = _db.Products.AsQueryable();

            if (request.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == request.CategoryId.Value);

            if (request.MinPrice.HasValue)
                query = query.Where(p => p.Price >= request.MinPrice.Value);

            if (request.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= request.MaxPrice.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(term) ||
                    p.Description.ToLower().Contains(term));
            }

            int skip = offset;
            int take = limit;

            return await query
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Product>> GetOnlySubscriptions(CancellationToken ct)
                => await _db.Products
                            .Where(p => p.Interval != Domain.Enums.IntervalEnum.OneTime)
                            .ToListAsync(ct);
    }
}
