using Dynamiq.Application.DTOs.PaymentDTOs;

namespace Dynamiq.Application.Interfaces.Stripe
{
    public interface IStripeCheckoutSession
    {
        Task<string> CreateCheckoutSessionAsync(CheckoutSessionDto request, List<StripeCartItemDto> cartItems, List<string>? couponCodes);
    }
}
