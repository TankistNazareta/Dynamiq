using Dynamiq.Application.DTOs.ProductDTOs;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Common;
using Dynamiq.Domain.Enums;
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
        public async Task<ResponseProducts> GetAllAsync(int limit, int offset, CancellationToken ct)
        {
            var query = _db.Products
                .AsNoTracking()
                .Where(p => p.Interval == Domain.Enums.IntervalEnum.OneTime);

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .Skip(offset)
                .Take(limit)
                .ToListAsync(ct);

            return new ResponseProducts(totalCount, items);
        }


        public async Task<ResponseProducts> GetAllBySlugAsync(string slug, int limit, int offset, CancellationToken ct)
        {
            var category = await _db.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Slug == slug, ct);

            var products = category?.Products.AsQueryable() ?? Enumerable.Empty<Product>().AsQueryable();

            var query = products.AsNoTracking()
                .Where(p => p.Interval == IntervalEnum.OneTime);

            var items = await query
                .Skip(offset)
                .Take(limit)
                .ToListAsync(ct);

            return new(await query.CountAsync(), items);
        }

        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct)
                => await _db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task<ResponseProducts> GetFilteredAsync(ProductFilter request, int limit, int offset, CancellationToken ct)
        {
            var query = _db.Products.AsQueryable();

            if (request.CategoryIds != null && request.CategoryIds.Count != 0)
            {
                query = query.Where(p => request.CategoryIds.Contains(p.CategoryId));
            }

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

            if (request.SortBy != null && request.SortBy != SortEnum.Default)
            {
                if (request.SortBy == SortEnum.FromLowest)
                    query = query.OrderBy(p => p.Price);

                if (request.SortBy == SortEnum.FromHighest)
                    query = query.OrderByDescending(p => p.Price);
            }


            var items = await query
                .Skip(offset)
                .Take(limit)
                .ToListAsync(ct);

            return new(await query.CountAsync(), items);
        }

        public async Task<IReadOnlyList<Product>> GetOnlySubscriptions(CancellationToken ct)
                => await _db.Products
                            .Where(p => p.Interval != Domain.Enums.IntervalEnum.OneTime)
                            .ToListAsync(ct);
    }
}
