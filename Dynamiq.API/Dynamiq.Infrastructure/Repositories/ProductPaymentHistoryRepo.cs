using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Domain.Entities;
using Dynamiq.Infrastructure.Persistence.Context;

namespace Dynamiq.Infrastructure.Repositories
{
    public class ProductPaymentHistoryRepo : IProductPaymentHistoryRepo
    {
        private readonly AppDbContext _db;

        public ProductPaymentHistoryRepo(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(ProductPaymentHistory productPaymentHistory, CancellationToken ct)
            => await _db.ProductPaymentHistories.AddAsync(productPaymentHistory, ct);
    }
}
