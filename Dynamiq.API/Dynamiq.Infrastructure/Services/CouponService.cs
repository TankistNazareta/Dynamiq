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

        public async Task<List<StripeCartItemDto>> AddAllCouponsAsync(List<StripeCartItemDto> cartItems, List<string> codes, CancellationToken ct)
        {
            var couponsWithFixedPrice = new List<int>();
            var couponsWithPercentage = new List<int>();

            foreach (var code in codes)
            {
                var coupon = await _rpeo.GetByCodeAsync(code, ct);

                if (coupon == null)
                    throw new KeyNotFoundException($"coupon with code {code}, wasn't found");

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
                    item.Price -= (item.Price * percent / 100);
                    if (item.Price < 0) item.Price = 0;
                }
            }

            // Fixed Discounts
            foreach (var fixedValue in couponsWithFixedPrice)
            {
                var remainingDiscount = fixedValue;

                foreach (var item in cartItems)
                {
                    if (remainingDiscount <= 0)
                        break;

                    if (remainingDiscount >= item.Price)
                    {
                        remainingDiscount -= item.Price;
                        item.Price = 0;
                    }
                    else
                    {
                        item.Price -= remainingDiscount;
                        remainingDiscount = 0;
                    }
                }
            }

            return cartItems;
        }
    }
}
