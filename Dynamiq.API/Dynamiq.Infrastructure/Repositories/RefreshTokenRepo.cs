using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Interfaces.Repositories;
using Dynamiq.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dynamiq.Infrastructure.Repositories
{
    public class RefreshTokenRepo : IRefreshTokenRepo
    {
        private readonly AppDbContext _db;

        public RefreshTokenRepo(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(RefreshToken token, CancellationToken ct)
                => await _db.RefreshTokens.AddAsync(token, ct);

        public void Delete(RefreshToken token)
                => _db.RefreshTokens.Remove(token);

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct)
                => await _db.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token, ct);

        public async Task<RefreshToken?> GetByUserIdAsync(Guid userId, CancellationToken ct)
                => await _db.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId, ct);
    }
}
