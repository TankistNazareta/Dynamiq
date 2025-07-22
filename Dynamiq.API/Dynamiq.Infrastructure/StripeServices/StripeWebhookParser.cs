using Dynamiq.Application.DTOs;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Shared.DTOs;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using System.Text.Json;

namespace Dynamiq.Infrastructure.StripeServices
{
    public class StripeWebhookParser : IStripeWebhookParser
    {
        private readonly string _webhookSecret;

        public StripeWebhookParser(IConfiguration config)
        {
            _webhookSecret = config["Stripe:WebhookSecret"];
        }

        public PaymentHistoryDto ParseCheckoutSessionCompleted(string json, string signature)
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                signature,
                _webhookSecret
            );

            if (stripeEvent.Type != "checkout.session.completed")
                return null;

            var session = stripeEvent.Data.Object as Session;

            if (session?.Metadata == null ||
                !session.Metadata.TryGetValue("PaymentHistoryDtoJson", out var paymentHistoryDtoJson))
                throw new KeyNotFoundException("Metadata 'PaymentHistoryDtoJson' not found in session.");

            var paymentHistoryDto = JsonSerializer.Deserialize<PaymentHistoryDto>(paymentHistoryDtoJson);

            if (paymentHistoryDto == null)
                throw new InvalidDataException("Failed to deserialize PaymentHistoryDtoJson.");

            paymentHistoryDto.Amount = session.AmountTotal.HasValue ? session.AmountTotal.Value / 100m : 0m;
            paymentHistoryDto.StripePaymentId = session.PaymentIntentId;

            return paymentHistoryDto;
        }
    }
}
