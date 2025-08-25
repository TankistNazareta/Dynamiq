using Dynamiq.Application.DTOs.PaymentDTOs;

namespace Dynamiq.Application.Interfaces.Services
{
    public interface ICouponService
    {
        Task<List<StripeCartItemDto>> AddAllCouponsAsync(List<StripeCartItemDto> cartItems, List<string> codes, CancellationToken ct);
    }
}
