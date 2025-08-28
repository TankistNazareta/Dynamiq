using Dynamiq.Application.Interfaces.Stripe;
using Microsoft.Extensions.Configuration;
using Stripe;
using StripeSdk = Stripe;

namespace Dynamiq.Infrastructure.Services.Stripe
{
    public class StripeCouponService : IStripeCouponService
    {
        private readonly string _stripeSecretKey;

        public StripeCouponService(IConfiguration configuration)
        {
            _stripeSecretKey = configuration["Stripe:SecretKey"]
                ?? throw new ArgumentNullException("Stripe:SecretKey is not configured");

            StripeConfiguration.ApiKey = _stripeSecretKey;
        }

        public async Task<string> CreateStripeCouponAsync(double discountAmount)
        {
            var service = new StripeSdk.CouponService();

            var options = new CouponCreateOptions
            {
                AmountOff = (long?)(discountAmount * 100),
                Currency = "usd",
                Duration = "once"
            };

            var coupon = await service.CreateAsync(options);
            return coupon.Id;
        }

        public async Task DeactivateCoupon(string stripeCouponId)
        {
            var service = new StripeSdk.CouponService();

            await service.DeleteAsync(stripeCouponId);
        }
    }
}
