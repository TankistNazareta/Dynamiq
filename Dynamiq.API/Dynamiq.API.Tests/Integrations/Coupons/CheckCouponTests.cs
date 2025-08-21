using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Enums;
using Dynamiq.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Dynamiq.API.Tests.Integrations.Coupons
{
    public class CheckCouponTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public CheckCouponTests(CustomWebApplicationFactory<Program> factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
            _factory = factory;
        }

        [Fact]
        public async Task CheckIfActiveByCode_ShouldReturnTrue_ForActiveCoupon()
        {
            var code = $"ACTIVE-{Guid.NewGuid():N}";

            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetService<AppDbContext>();

                var coupon = new Coupon(
                    code,
                    DiscountTypeEnum.FixedAmount,
                    100,
                    DateTime.UtcNow.AddHours(-1),
                    DateTime.UtcNow.AddHours(1)
                );

                db.Coupons.Add(coupon);

                await db.SaveChangesAsync();
            }

            var client = _factory.CreateClient();
            var response = await client.GetAsync($"/coupon/CheckIfActiveByCode?code={code}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CheckIfActiveByCode_ShouldReturnNotFound_ForNonExistingCoupon()
        {
            var code = $"UNKNOWN-{Guid.NewGuid():N}";

            var client = _factory.CreateClient();
            var response = await client.GetAsync($"/coupon/CheckIfActiveByCode?code={code}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CheckIfActiveByCode_ShouldReturnFalse_ForDeactivatedCoupon()
        {
            var code = $"INACTIVE-{Guid.NewGuid():N}";

            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetService<AppDbContext>();

                var coupon = new Coupon(
                    code,
                    DiscountTypeEnum.FixedAmount,
                    100,
                    DateTime.UtcNow.AddHours(-1),
                    DateTime.UtcNow.AddHours(1)
                );

                coupon.DeactivateCoupon();

                db.Coupons.Add(coupon);

                await db.SaveChangesAsync();
            }

            var client = _factory.CreateClient();
            var response = await client.GetAsync($"/coupon/CheckIfActiveByCode?code={code}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
