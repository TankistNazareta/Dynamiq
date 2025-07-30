using Dynamiq.Application.DTOs;
using Dynamiq.Shared.DTOs;

namespace Dynamiq.Application.Interfaces.Stripe
{
    public interface IStripeWebhookParser
    {
        WebhookParserDto ParseCheckoutSessionCompleted(string json, string signature);
    }
}
