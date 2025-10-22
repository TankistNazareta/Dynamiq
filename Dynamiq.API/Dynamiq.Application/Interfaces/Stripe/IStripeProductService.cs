using Dynamiq.Application.DTOs.ProductDTOs;
using Dynamiq.Application.DTOs.PaymentDTOs;
using Dynamiq.Domain.Enums;

namespace Dynamiq.Application.Interfaces.Stripe
{
    public interface IStripeProductService
    {
        Task<StripeIdsDto> CreateProductStripeAsync(ProductDto product, IntervalEnum? intervalEnum = null);
        Task<StripeIdsDto> UpdateProductStripeAsync(ProductDto product, string stripeProductId, string stripePriceId, IntervalEnum? intervalEnum = null);
        Task DeleteProductStripeAsync(string priceId, string productId);
    }
}