using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Domain.Aggregates;
using Dynamiq.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dynamiq.Infrastructure.Repositories
{
    public class PaymentHistoryRepo : IPaymentHistoryRepo
    {
        private readonly AppDbContext _db;

        public PaymentHistoryRepo(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(PaymentHistory paymentHistory, CancellationToken ct)
                => await _db.PaymentHistories.AddAsync(paymentHistory, ct);

        public async Task<IReadOnlyList<PaymentHistory>> GetListByUserIdAsync(Guid userId, CancellationToken ct)
                => await _db.PaymentHistories
                        .AsNoTracking()
                        .Where(ph => ph.UserId == userId)
                        .ToListAsync(ct);
    }
}
