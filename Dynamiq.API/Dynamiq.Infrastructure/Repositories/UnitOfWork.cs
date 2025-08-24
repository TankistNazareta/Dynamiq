using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Infrastructure.Persistence.Context;

namespace Dynamiq.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;

        public UnitOfWork(AppDbContext db)
        {
            _db = db;
        }

        public async Task SaveChangesAsync(CancellationToken ct)
                => await _db.SaveChangesAsync(ct);
    }
}
