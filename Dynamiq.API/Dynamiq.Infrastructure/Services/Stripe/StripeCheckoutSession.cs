using Dynamiq.Application.DTOs;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using System.Text.Json;

namespace Dynamiq.Infrastructure.Services.Stripe
{
    public class StripeCheckoutSession : IStripeCheckoutSession
    {
        private readonly StripeClient _client;

        private readonly string _stripeSecretKey;

        public StripeCheckoutSession(IConfiguration config)
        {
            _stripeSecretKey = config["Stripe:SecretKey"]!;

            _client = new StripeClient(_stripeSecretKey);
        }

        public async Task<string> CreateCheckoutSessionAsync(CheckoutSessionDto request, List<StripeCartItemDto> cartItems)
        {
            string ModeType = "subscription";

            if (request.Interval == IntervalEnum.OneTime)
                ModeType = "payment";

            var optionsItems = new List<SessionLineItemOptions>();

            foreach (var cartItem in cartItems)
            {
                optionsItems.Add(new SessionLineItemOptions()
                {
                    Price = cartItem.StripePriceId,
                    Quantity = cartItem.Quantity
                });
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
                    { "WebhookParserDto",  metaDataJson}
                }
            };

            var service = new SessionService(_client);
            var session = await service.CreateAsync(options);

            return session.Url;
        }
    }
}
