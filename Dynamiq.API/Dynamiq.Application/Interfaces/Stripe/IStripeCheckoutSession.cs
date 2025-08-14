using Dynamiq.Application.DTOs.StripeDTOs;

namespace Dynamiq.Application.Interfaces.Stripe
{
    public interface IStripeCheckoutSession
    {
        Task<string> CreateCheckoutSessionAsync(CheckoutSessionDto request, List<StripeCartItemDto> cartItems);
    }
}
