namespace Dynamiq.Application.Interfaces.UseCases
{
    public interface IUserCleanupUseCase
    {
        Task<int> RemoveAllExpiredUsersAsync(CancellationToken cancellationToken);
    }
}
