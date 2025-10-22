using Dynamiq.Application.DTOs.PaymentDTOs;

namespace Dynamiq.Application.Interfaces.Stripe
{
    public interface IStripeWebhookParser
    {
        WebhookParserDto ParseCheckoutSessionCompleted(string json, string signature);
        string GetEventType(string json, string signature);
        CouponsResultDto? TryGetCoupons(string json, string signature);
        string ParseDeletedSubscriptionId(string json, string signature);
    }
}
