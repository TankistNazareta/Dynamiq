using Dynamiq.Domain.Aggregates;

namespace Dynamiq.Domain.Interfaces.Repositories
{
    public interface IProductRepo
    {
        Task AddAsync(Product product, CancellationToken ct);
        Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct);
        Task<Product?> GetByIdAsync(Guid id, CancellationToken ct);
        void Delete(Product product);
        Task<IReadOnlyList<Product>> GetAllBySlugAsync(string slug, CancellationToken ct);
        Task<IReadOnlyList<Product>> GetOnlySubscriptions(CancellationToken ct);
    }
}