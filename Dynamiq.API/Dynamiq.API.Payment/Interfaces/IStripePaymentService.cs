using Dynamiq.API.Extension.RequestEntity;
using Dynamiq.API.Mapping.DTOs;

namespace Dynamiq.API.Stripe.Interfaces
{
    public interface IStripePaymentService
    {
        Task<string> CreateCheckoutSession(CheckoutSessionRequest request);
        Task<PaymentHistoryDto> StripeWebhook(string stripeResponseJson, string stripeSignatureHeader);
    }
}
