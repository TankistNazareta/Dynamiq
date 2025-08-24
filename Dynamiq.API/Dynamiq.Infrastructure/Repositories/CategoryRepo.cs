using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Domain.Aggregates;
using Dynamiq.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dynamiq.Infrastructure.Repositories
{
    public class CategoryRepo : ICategoryRepo
    {
        private readonly AppDbContext _db;

        public CategoryRepo(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken)
                    => await _db.Categories.AsNoTracking().ToListAsync(cancellationToken);

        public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
                    => await _db.Categories.AsNoTracking().FirstOrDefaultAsync(cate => cate.Id == id);
    }
}
