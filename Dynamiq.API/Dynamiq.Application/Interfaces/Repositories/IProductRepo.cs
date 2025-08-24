using Dynamiq.Application.DTOs.ProductDTOs;
using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Common;

namespace Dynamiq.Application.Interfaces.Repositories
{
    public interface IProductRepo
    {
        Task AddAsync(Product product, CancellationToken ct);
        Task<ResponseProducts
            > GetAllAsync(int limit, int offset, CancellationToken ct);
        Task<Product?> GetByIdAsync(Guid id, CancellationToken ct);
        void Delete(Product product);
        Task<ResponseProducts> GetAllBySlugAsync(string slug, int limit, int offset, CancellationToken ct);
        Task<IReadOnlyList<Product>> GetOnlySubscriptions(CancellationToken ct);
        Task<ResponseProducts> GetFilteredAsync(ProductFilter request, int limit, int offset, CancellationToken ct);
    }
}