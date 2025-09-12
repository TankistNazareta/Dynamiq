using Dynamiq.Application.DTOs.PaymentDTOs;
using Dynamiq.Application.Interfaces.Services;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using System.Text.Json;
using System.Threading;

namespace Dynamiq.Infrastructure.Services.Stripe
{
    public class StripeCheckoutSession : IStripeCheckoutSession
    {
        private readonly IStripeCouponService _couponServiceStripe;
        private readonly ICouponService _couponService;

        private readonly StripeClient _client;

        private readonly string _stripeSecretKey;

        public StripeCheckoutSession(IConfiguration config, IStripeCouponService couponServiceStripe, ICouponService couponService)
        {
            _couponServiceStripe = couponServiceStripe;
            _couponService = couponService;

            _stripeSecretKey = config["Stripe:SecretKey"]!;

            _client = new StripeClient(_stripeSecretKey);
        }

        public async Task<string> CreateCheckoutSessionAsync(CheckoutSessionDto request, List<StripeCartItemDto> cartItems, List<string> coupons, CancellationToken ct)
        {
            string ModeType = "subscription";

            if (request.Interval == IntervalEnum.OneTime)
                ModeType = "payment";

            var optionsItems = new List<SessionLineItemOptions>();
            var stripeCoupons = new List<SessionDiscountOptions>();
            var stripeCouponIds = new List<string>();

            foreach (var cartItem in cartItems)
            {
                optionsItems.Add(new SessionLineItemOptions()
                {
                    Price = cartItem.StripePriceId,
                    Quantity = cartItem.Quantity
                });
            }

            if (coupons != null && coupons.Count != 0)
            {
                var totalDiscount = await _couponService.CalculateTotalDiscount(cartItems, coupons, ct);

                var stripeCouponId = await _couponServiceStripe.CreateStripeCouponAsync((double)totalDiscount);

                stripeCoupons.Add(new SessionDiscountOptions()
                {
                    Coupon = stripeCouponId,
                });

                stripeCouponIds.Add(stripeCouponId);
            }

            var metaData = new WebhookParserDto()
            {
                UserId = request.UserId,
                ProductId = request.ProductId,
                Interval = request.Interval,
            };

            var metaDataJson = JsonSerializer.Serialize(metaData);

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = optionsItems,
                Mode = ModeType,
                SuccessUrl = request.SuccessUrl,
                CancelUrl = request.CancelUrl,

                Metadata = new Dictionary<string, string>
                {
                    { "WebhookParserDto",  metaDataJson},
                    { "Coupons", JsonSerializer.Serialize(coupons) },
                    { "CouponStripeIds", JsonSerializer.Serialize(stripeCouponIds) }
                },
            };

            if (stripeCoupons.Any())
            {
                options.Discounts = stripeCoupons;
            }

            var service = new SessionService(_client);
            var session = await service.CreateAsync(options);

            return session.Url;
        }
    }
}
