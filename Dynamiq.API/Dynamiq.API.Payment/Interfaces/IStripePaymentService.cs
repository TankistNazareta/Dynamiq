using Dynamiq.API.Extension.RequestEntity;

namespace Dynamiq.API.Stripe.Interfaces
{
    public interface IStripePaymentService
    {
        Task<string> CreateCheckoutSession(CheckoutSessionRequest request);
        Task StripeWebhook(string stripeResponseJson, string stripeSignatureHeader);
    }
}
