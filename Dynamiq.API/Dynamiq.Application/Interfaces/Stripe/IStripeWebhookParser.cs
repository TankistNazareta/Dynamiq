using Dynamiq.Application.DTOs.PaymentDTOs;

namespace Dynamiq.Application.Interfaces.Stripe
{
    public interface IStripeWebhookParser
    {
        WebhookParserDto ParseCheckoutSessionCompleted(string json, string signature, out string eventType);
        CouponsResultDto? TryGetCoupons(string json, string signature);
    }
}
