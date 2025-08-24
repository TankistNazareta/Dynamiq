using Dynamiq.Domain.Aggregates;

namespace Dynamiq.Application.Interfaces.Repositories
{
    public interface ICategoryRepo
    {
        Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken);
    }
}
