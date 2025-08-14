using Dynamiq.Application.DTOs.ProductDTOs;
using Dynamiq.Application.DTOs.StripeDTOs;

namespace Dynamiq.Application.Interfaces.Stripe
{
    public interface IStripeProductService
    {
        Task<StripeIdsDto> CreateProductStripeAsync(ProductDto product);
        Task<StripeIdsDto> UpdateProductStripeAsync(ProductDto product, string stripeProductId, string stripePriceId);
        Task DeleteProductStripeAsync(string priceId, string productId);
    }
}