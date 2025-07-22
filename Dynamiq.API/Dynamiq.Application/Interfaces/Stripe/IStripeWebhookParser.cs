using Dynamiq.Application.DTOs;
using Dynamiq.Shared.DTOs;

namespace Dynamiq.Application.Interfaces.Stripe
{
    public interface IStripeWebhookParser
    {
        PaymentHistoryDto ParseCheckoutSessionCompleted(string json, string signature);
    }
}
