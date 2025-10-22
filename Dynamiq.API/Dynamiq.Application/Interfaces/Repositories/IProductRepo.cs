using Dynamiq.Application.DTOs.ProductDTOs;
using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Common;

namespace Dynamiq.Application.Interfaces.Repositories
{
    public interface IProductRepo
    {
        Task AddAsync(Product product, CancellationToken ct);
        Task<ResponseProducts> GetAllAsync(int limit, int offset, CancellationToken ct);
        Task<Product?> GetByIdAsync(Guid id, CancellationToken ct);
        void Delete(Product product);
        Task<ResponseProducts> GetAllBySlugAsync(string slug, int limit, int offset, CancellationToken ct);
        Task<ResponseProducts> GetFilteredAsync(ProductFilter filter, int limit, int offset, CancellationToken ct);
        Task<List<string>> GetFilteredNamesAsync(ProductFilter filter, int limit, CancellationToken ct);
    }
}