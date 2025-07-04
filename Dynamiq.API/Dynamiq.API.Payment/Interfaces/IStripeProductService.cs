using Dynamiq.API.Extension.RequestEntity;
using Dynamiq.API.Mapping.DTOs;

namespace Dynamiq.API.Stripe.Interfaces
{
    public interface IStripeProductService
    {
        Task<ProductDto> CreateProductStripe(ProductRequestEntity product);
        Task<ProductDto> UpdateProductStripe(ProductDto product);
        Task DeleteProductStripe(string priceId, string productId);
    }
}