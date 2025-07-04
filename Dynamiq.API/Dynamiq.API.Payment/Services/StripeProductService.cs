using Dynamiq.API.Extension.RequestEntity;
using Dynamiq.API.Mapping.DTOs;
using Dynamiq.API.Stripe.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Dynamiq.API.Stripe.Services
{
    public class StripeProductService : IStripeProductService
    {
        private readonly StripeClient _client;

        public StripeProductService(IConfiguration config)
        {
            var secretKey = config["Stripe:SecretKey"];
            _client = new StripeClient(secretKey);
        }

        public async Task<ProductDto> CreateProductStripe(ProductRequestEntity product)
        {
            var productService = new ProductService(_client);
            var priceService = new PriceService(_client);

            var createdProduct = await productService.CreateAsync(new ProductCreateOptions
            {
                Name = product.Name,
                Description = product.Description,
            });

            var createdPrice = await priceService.CreateAsync(new PriceCreateOptions
            {
                UnitAmount = product.Price * 100,
                Currency = "usd",
                Product = createdProduct.Id,
            });

            return new()
            {
                StripeProductId = createdProduct.Id,
                StripePriceId = createdPrice.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
            };
        }

        public async Task<ProductDto> UpdateProductStripe(ProductDto product)
        {
            var productService = new ProductService(_client);
            var priceService = new PriceService(_client);

            var updatedProduct = await productService.UpdateAsync(product.StripeProductId, new ProductUpdateOptions
            {
                Name = product.Name,
                Description = product.Description,
            });

            await productService.UpdateAsync(product.StripePriceId, new ProductUpdateOptions
            {
                Active = false
            });

            var createdPrice = await priceService.CreateAsync(new PriceCreateOptions
            {
                UnitAmount = product.Price,
                Currency = "usd",
                Product = updatedProduct.Id,
            });

            return new()
            {
                StripeProductId = updatedProduct.Id,
                StripePriceId = createdPrice.Id,
                Name = updatedProduct.Name,
                Price = product.Price 
            };
        }

        public async Task DeleteProductStripe(string priceId, string productId)
        {
            var productService = new ProductService(_client);
            var priceService = new PriceService(_client);

            await priceService.UpdateAsync(priceId, new PriceUpdateOptions
            {
                Active = false
            });

            await productService.UpdateAsync(productId, new ProductUpdateOptions
            {
                Active = false
            });
        }
    }
}
