using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Enums;
using FluentAssertions;

namespace Dynamiq.Domain.Tests.Entities
{
    public class CouponTests
    {
        [Fact]
        public void Coupon_ShouldInitializeCorrectly()
        {
            var code = "TESTCODE";
            var discountType = DiscountTypeEnum.FixedAmount;
            var discountValue = 100;
            var startTime = DateTime.UtcNow.AddHours(-1);
            var endTime = DateTime.UtcNow.AddHours(1);

            var coupon = new Coupon(code, discountType, discountValue, startTime, endTime);

            coupon.Code.Should().Be(code);
            coupon.DiscountType.Should().Be(discountType);
            coupon.DiscountValue.Should().Be(discountValue);
            coupon.StartTime.Should().Be(startTime);
            coupon.EndTime.Should().Be(endTime);
            coupon.IsActiveCoupon.Should().BeTrue();
        }

        [Fact]
        public void IsActive_ShouldReturnTrue_WhenCouponIsActiveAndNotExpired()
        {
            var coupon = new Coupon(
                "TEST",
                DiscountTypeEnum.FixedAmount,
                50,
                DateTime.UtcNow.AddHours(-1),
                DateTime.UtcNow.AddHours(1)
            );

            var result = coupon.IsActive();

            result.Should().BeTrue();
        }

        [Fact]
        public void IsActive_ShouldReturnFalse_WhenCouponIsDeactivated()
        {
            var coupon = new Coupon(
                "TEST",
                DiscountTypeEnum.FixedAmount,
                50,
                DateTime.UtcNow.AddHours(-1),
                DateTime.UtcNow.AddHours(1)
            );
            coupon.DeactivateCoupon();

            var result = coupon.IsActive();

            result.Should().BeFalse();
        }

        [Fact]
        public void IsActive_ShouldReturnFalse_WhenCouponIsExpired()
        {
            var coupon = new Coupon(
                "TEST",
                DiscountTypeEnum.FixedAmount,
                50,
                DateTime.UtcNow.AddHours(-2),
                DateTime.UtcNow.AddHours(-1)
            );

            var result = coupon.IsActive();

            result.Should().BeFalse();
        }

        [Fact]
        public void DeactivateCoupon_ShouldSetIsActiveCouponToFalse()
        {
            var coupon = new Coupon(
                "TEST",
                DiscountTypeEnum.FixedAmount,
                50,
                DateTime.UtcNow.AddHours(-1),
                DateTime.UtcNow.AddHours(1)
            );

            coupon.DeactivateCoupon();

            coupon.IsActiveCoupon.Should().BeFalse();
        }
    }
}
