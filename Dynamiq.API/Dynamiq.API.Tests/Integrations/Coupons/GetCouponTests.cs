using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Enums;
using Dynamiq.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Dynamiq.API.Tests.Integrations.Coupons
{
    public class GetCouponTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public GetCouponTests(CustomWebApplicationFactory<Program> factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
            _factory = factory;
        }

        [Fact]
        public async Task GetCouponByCode_ShouldReturnOK()
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
            var response = await client.GetAsync($"/coupon?code={code}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetCouponByCode_ShouldReturnNotFound()
        {
            var code = $"UNKNOWN-{Guid.NewGuid():N}";

            var client = _factory.CreateClient();
            var response = await client.GetAsync($"/coupon?code={code}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
