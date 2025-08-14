using Dynamiq.Application.DTOs.StripeDTOs;

namespace Dynamiq.Application.Interfaces.Stripe
{
    public interface IStripeWebhookParser
    {
        WebhookParserDto ParseCheckoutSessionCompleted(string json, string signature);
    }
}
