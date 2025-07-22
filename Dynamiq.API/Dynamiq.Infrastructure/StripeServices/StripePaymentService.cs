using Dynamiq.Application.DTOs;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using System.Text.Json;

namespace Dynamiq.Infrastructure.StripeServices
{
    public class StripePaymentService : IStripePaymentService
    {
        private readonly StripeClient _client;

        private readonly string _stripeSecretKey;

        public StripePaymentService(IConfiguration config)
        {
            _stripeSecretKey = config["Stripe:SecretKey"]!;

            _client = new StripeClient(_stripeSecretKey);
        }

        public async Task<string> CreateCheckoutSessionAsync(CheckoutSessionDto request)
        {
            string ModeType = "subscription";

            if (request.Interval == IntervalEnum.OneTime)
                ModeType = "payment";

            var paymentHistoryDto = new PaymentHistoryDto()
            {
                ProductId = request.ProductId,
                UserId = request.UserId,
                Interval = request.Interval
            };

            var paymentHistoryDtoJson = JsonSerializer.Serialize(paymentHistoryDto);

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = request.StripePriceId,
                        Quantity = request.Quantity
                    }
                },
                Mode = ModeType,
                SuccessUrl = request.SuccessUrl,
                CancelUrl = request.CancelUrl,

                Metadata = new Dictionary<string, string>
                {
                    { "PaymentHistoryDtoJson", paymentHistoryDtoJson }
                }
            };

            var service = new SessionService(_client);
            var session = await service.CreateAsync(options);

            return session.Url;
        }
    }
}
