using Dynamiq.Application.Queries.Coupons.Queries;
using Dynamiq.Domain.Interfaces.Repositories;
using MediatR;

namespace Dynamiq.Application.Queries.Coupons.Handlers
{
    public class CheckIfCouponIsActiveHandler : IRequestHandler<CheckIfCouponIsActiveQuery, bool>
    {
        private readonly ICouponRepo _couponRepo;

        public CheckIfCouponIsActiveHandler(ICouponRepo couponRepo)
        {
            _couponRepo = couponRepo;
        }

        public async Task<bool> Handle(CheckIfCouponIsActiveQuery request, CancellationToken cancellationToken)
        {
            var coupon = await _couponRepo.GetByCodeAsync(request.Code, cancellationToken);

            if (coupon == null)
                throw new KeyNotFoundException("Cupon wasn't found");

            return coupon.IsActive();
        }
    }
}
