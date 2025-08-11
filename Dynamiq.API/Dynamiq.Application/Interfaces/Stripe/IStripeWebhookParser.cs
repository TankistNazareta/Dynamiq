using Dynamiq.Application.DTOs;

namespace Dynamiq.Application.Interfaces.Stripe
{
    public interface IStripeWebhookParser
    {
        WebhookParserDto ParseCheckoutSessionCompleted(string json, string signature);
    }
}
