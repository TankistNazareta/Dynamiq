using Dynamiq.Application.DTOs.PaymentDTOs;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Application.Interfaces.Services;
using Dynamiq.Domain.Enums;

namespace Dynamiq.Infrastructure.Services
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepo _rpeo;

        public CouponService(ICouponRepo repo)
        {
            _rpeo = repo;
        }

        public async Task<double> CalculateTotalDiscount(List<StripeCartItemDto> cartItems, List<string> codes, CancellationToken ct)
        {
            var couponsWithFixedPrice = new List<int>();
            var couponsWithPercentage = new List<int>();

            var totalDiscount = 0.0;

            foreach (var code in codes)
            {
                var coupon = await _rpeo.GetByCodeAsync(code, ct);

                if (coupon == null)
                    throw new KeyNotFoundException($"coupon with code {code}, wasn't found");

                if (!coupon.IsActive())
                    throw new TimeoutException("coupon has expired.");

                switch (coupon.DiscountType)
                {
                    case DiscountTypeEnum.Percentage:
                        couponsWithPercentage.Add(coupon.DiscountValue);
                        break;
                    case DiscountTypeEnum.FixedAmount:
                        couponsWithFixedPrice.Add(coupon.DiscountValue);
                        break;
                }
            }

            // Percent Discounts
            foreach (var percent in couponsWithPercentage)
            {
                foreach (var item in cartItems)
                {
                    totalDiscount += (item.Price * percent / 100) * item.Quantity;
                }
            }

            // Fixed Discounts
            foreach (var fixedValue in couponsWithFixedPrice)
            {
                totalDiscount += fixedValue;
            }

            return totalDiscount;
        }
    }
}
