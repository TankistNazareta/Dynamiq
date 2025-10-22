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

        public async Task<ResponseProducts> GetAllAsync(int limit, int offset, CancellationToken ct)
        {
            var query = _db.Products
                .AsNoTracking();

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

            var query = products.AsNoTracking();

            var items = await query
                .Skip(offset)
                .Take(limit)
                .ToListAsync(ct);

            return new(await query.CountAsync(), items);
        }

        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct)
                => await _db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task<ResponseProducts> GetFilteredAsync(ProductFilter filter, int limit, int offset, CancellationToken ct)
        {
            var query = _db.Products.AsQueryable();

            query = FilterProducts(filter, query);

            var items = await query
                .Skip(offset)
                .Take(limit)
                .ToListAsync(ct);

            return new(await query.CountAsync(), items);
        }

        public async Task<List<string>> GetFilteredNamesAsync(ProductFilter filter, int limit, CancellationToken ct)
        {
            var query = _db.Products.AsQueryable();

            query.OrderBy(x => x.ViewCount);

            query = FilterProducts(filter, query);

            var names = await query
                .Take(limit)
                .Select(p => p.Name)
                .ToListAsync(ct);

            return names;
        }

        private IQueryable<Product> FilterProducts(ProductFilter filter, IQueryable<Product> query)
        {
            if (filter.CategoryIds != null && filter.CategoryIds.Count != 0)
            {
                query = query.Where(p => filter.CategoryIds.Contains(p.CategoryId));
            }
            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);
            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(term) ||
                    p.Description.ToLower().Contains(term));
            }
            if (filter.SortBy != null && filter.SortBy != SortEnum.Default)
            {
                if (filter.SortBy == SortEnum.FromLowest)
                    query = query.OrderBy(p => p.Price);
                if (filter.SortBy == SortEnum.FromHighest)
                    query = query.OrderByDescending(p => p.Price);
            }
            return query;
        }
    }
}
