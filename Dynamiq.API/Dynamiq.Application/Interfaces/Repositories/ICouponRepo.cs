using Dynamiq.Domain.Entities;

namespace Dynamiq.Application.Interfaces.Repositories
{
    public interface ICouponRepo
    {
        Task<Coupon?> GetByCodeAsync(string code, CancellationToken ct);
        Task AddAsync(Coupon coupon, CancellationToken ct);
    }
}
