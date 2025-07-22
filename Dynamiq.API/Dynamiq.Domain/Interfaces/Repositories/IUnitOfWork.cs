namespace Dynamiq.Domain.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken ct);
    }
}
