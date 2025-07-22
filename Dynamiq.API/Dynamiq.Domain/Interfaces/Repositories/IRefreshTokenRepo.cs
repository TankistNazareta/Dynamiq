using Dynamiq.Domain.Entities;

namespace Dynamiq.Domain.Interfaces.Repositories
{
    public interface IRefreshTokenRepo
    {
        Task AddAsync(RefreshToken token, CancellationToken ct);
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct);
        Task<RefreshToken?> GetByUserIdAsync(Guid userId, CancellationToken ct);
        void Delete(RefreshToken token);
    }
}
