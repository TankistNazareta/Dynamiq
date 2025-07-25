using Dynamiq.Domain.Aggregates;

namespace Dynamiq.Domain.Interfaces.Repositories
{
    public interface ICategoryRepo
    {
        Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken);
    }
}
