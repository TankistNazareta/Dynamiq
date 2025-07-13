using Dynamiq.API.Extension.Enums;
using Dynamiq.API.Extension.RequestEntity;
using Dynamiq.API.Mapping.DTOs;
using Dynamiq.API.Stripe.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using System.Text.Json;

namespace Dynamiq.API.Stripe.Services
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

        public async Task<string> CreateCheckoutSession(CheckoutSessionRequest request)
        {
            string ModeType;

            switch (request.PaymentTypeEnum)
            {
                case IntervalEnum.OneTime:
                    ModeType = "payment";
                    break;
                case IntervalEnum.Mountly:
                    ModeType = "subscription";
                    break;
                default:
                    throw new NotImplementedException("Unknown mode type: " + request.PaymentTypeEnum.ToString());
            }

            var paymentHistoryDto = new PaymentHistoryDto()
            {
                Interval = request.PaymentTypeEnum,
                UserId = request.UserId,
                ProductId = request.ProductId,
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
