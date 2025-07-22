using Dynamiq.Application.DTOs;

namespace Dynamiq.Application.Interfaces.Stripe
{
    public interface IStripePaymentService
    {
        Task<string> CreateCheckoutSessionAsync(CheckoutSessionDto request);
    }
}
