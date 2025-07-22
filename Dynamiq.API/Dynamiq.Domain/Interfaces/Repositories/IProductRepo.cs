using Dynamiq.Domain.Entities;

namespace Dynamiq.Domain.Interfaces.Repositories
{
    public interface IProductRepo
    {
        Task AddAsync(Product product, CancellationToken ct);
        Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct);
        Task<Product?> GetByIdAsync(Guid id, CancellationToken ct);
        void Delete(Product product);
    }
}