using Dynamiq.Application.DTOs;

namespace Dynamiq.Application.Interfaces.Stripe
{
    public interface IStripeCheckoutSession
    {
        Task<string> CreateCheckoutSessionAsync(CheckoutSessionDto request, List<StripeCartItemDto> cartItems);
    }
}
