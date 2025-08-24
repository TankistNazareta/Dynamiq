using Dynamiq.Application.DTOs.StripeDTOs;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Domain.Enums;
using Dynamiq.Infrastructure.Services;
using FluentAssertions;
using Moq;

namespace Dynamiq.Infrastructure.Tests.Services
{
    public class CouponServiceTests
    {
        private readonly Mock<ICouponRepo> _repoMock;
        private readonly CouponService _service;

        public CouponServiceTests()
        {
            _repoMock = new Mock<ICouponRepo>();
            _service = new CouponService(_repoMock.Object);
        }

        [Fact]
        public async Task AddAllCouponsAsync_ShouldApplyPercentageDiscountsCorrectly()
        {
            var cartItems = new List<StripeCartItemDto>
            {
                new StripeCartItemDto(Guid.NewGuid(), 1, "price_1", 1000)
            };

            var coupon = new Domain.Entities.Coupon(
                "PERC10",
                DiscountTypeEnum.Percentage,
                10,
                DateTime.UtcNow.AddHours(-1),
                DateTime.UtcNow.AddHours(1)
            );

            _repoMock.Setup(r => r.GetByCodeAsync("PERC10", It.IsAny<CancellationToken>()))
                     .ReturnsAsync(coupon);

            var result = await _service.AddAllCouponsAsync(cartItems, new List<string> { "PERC10" }, CancellationToken.None);

            result[0].Price.Should().Be(900); // 10% знижки
        }

        [Fact]
        public async Task AddAllCouponsAsync_ShouldApplyFixedDiscountsCorrectly()
        {
            var cartItems = new List<StripeCartItemDto>
            {
                new StripeCartItemDto(Guid.NewGuid(), 1, "price_1", 1000)
            };

            var coupon = new Domain.Entities.Coupon(
                "FIX500",
                DiscountTypeEnum.FixedAmount,
                500,
                DateTime.UtcNow.AddHours(-1),
                DateTime.UtcNow.AddHours(1)
            );

            _repoMock.Setup(r => r.GetByCodeAsync("FIX500", It.IsAny<CancellationToken>()))
                     .ReturnsAsync(coupon);

            var result = await _service.AddAllCouponsAsync(cartItems, new List<string> { "FIX500" }, CancellationToken.None);

            result[0].Price.Should().Be(500);
        }

        [Fact]
        public async Task AddAllCouponsAsync_ShouldCombinePercentageAndFixedDiscounts()
        {
            var cartItems = new List<StripeCartItemDto>
        {
            new StripeCartItemDto(Guid.NewGuid(), 1, "price_1", 1000)
        };

            var percCoupon = new Domain.Entities.Coupon(
                "PERC10",
                DiscountTypeEnum.Percentage,
                10,
                DateTime.UtcNow.AddHours(-1),
                DateTime.UtcNow.AddHours(1)
            );

            var fixedCoupon = new Domain.Entities.Coupon(
                "FIX200",
                DiscountTypeEnum.FixedAmount,
                200,
                DateTime.UtcNow.AddHours(-1),
                DateTime.UtcNow.AddHours(1)
            );

            _repoMock.Setup(r => r.GetByCodeAsync("PERC10", It.IsAny<CancellationToken>()))
                     .ReturnsAsync(percCoupon);

            _repoMock.Setup(r => r.GetByCodeAsync("FIX200", It.IsAny<CancellationToken>()))
                     .ReturnsAsync(fixedCoupon);

            var result = await _service.AddAllCouponsAsync(cartItems, new List<string> { "PERC10", "FIX200" }, CancellationToken.None);

            // Спочатку 10% => 1000 - 100 = 900
            // Потім fixed 200 => 900 - 200 = 700
            result[0].Price.Should().Be(700);
        }

        [Fact]
        public async Task AddAllCouponsAsync_ShouldThrow_WhenCouponNotFound()
        {
            _repoMock.Setup(r => r.GetByCodeAsync("UNKNOWN", It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Domain.Entities.Coupon)null);

            var cartItems = new List<StripeCartItemDto>
        {
            new StripeCartItemDto(Guid.NewGuid(), 1, "price_1", 1000)
        };

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.AddAllCouponsAsync(cartItems, new List<string> { "UNKNOWN" }, CancellationToken.None));
        }

        [Fact]
        public async Task AddAllCouponsAsync_ShouldNotSetNegativePrice()
        {
            var cartItems = new List<StripeCartItemDto>
            {
                new StripeCartItemDto(Guid.NewGuid(), 1, "price_1", 100)
            };

            var coupon = new Domain.Entities.Coupon(
                "FIX200",
                DiscountTypeEnum.FixedAmount,
                200,
                DateTime.UtcNow.AddHours(-1),
                DateTime.UtcNow.AddHours(1)
            );

            _repoMock.Setup(r => r.GetByCodeAsync("FIX200", It.IsAny<CancellationToken>()))
                     .ReturnsAsync(coupon);

            var result = await _service.AddAllCouponsAsync(cartItems, new List<string> { "FIX200" }, CancellationToken.None);

            result[0].Price.Should().Be(0);
        }
    }
}
