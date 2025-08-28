using Dynamiq.Application.DTOs.PaymentDTOs;

namespace Dynamiq.Application.Interfaces.Services
{
    public interface ICouponService
    {
        Task<double> CalculateTotalDiscount(List<StripeCartItemDto> cartItems, List<string> codes, CancellationToken ct);
    }
}
