using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Domain.Entities;
using Dynamiq.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dynamiq.Infrastructure.Repositories
{
    public class CouponRepo : ICouponRepo
    {
        private readonly AppDbContext _db;

        public CouponRepo(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Coupon coupon, CancellationToken ct)
            => await _db.Coupons.AddAsync(coupon);

        public async Task<Coupon?> GetByCodeAsync(string code, CancellationToken ct)
            => await _db.Coupons.FirstOrDefaultAsync(c => c.Code == code);
    }
}
