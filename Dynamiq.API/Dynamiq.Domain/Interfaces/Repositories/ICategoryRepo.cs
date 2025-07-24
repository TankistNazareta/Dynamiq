using Dynamiq.Domain.Entities;

namespace Dynamiq.Domain.Interfaces.Repositories
{
    public interface ICategoryRepo
    {
        Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken);
    }
}
