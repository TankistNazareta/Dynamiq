using Dynamiq.Application.DTOs;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Domain.Enums;
using Dynamiq.Shared.DTOs;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Dynamiq.Infrastructure.Services.Stripe
{
    public class StripeProductService : IStripeProductService
    {
        private readonly StripeClient _client;

        public StripeProductService(IConfiguration config)
        {
            var secretKey = config["Stripe:SecretKey"];
            _client = new StripeClient(secretKey);
        }

        public async Task<StripeIdsDto> CreateProductStripeAsync(ProductDto product)
        {
            var productService = new ProductService(_client);
            var priceService = new PriceService(_client);

            var recurring = CreatePriceRecurringOptions(product.Interval);

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
                Recurring = recurring
            });

            return new(createdPrice.Id, createdProduct.Id);
        }

        public async Task<StripeIdsDto> UpdateProductStripeAsync(ProductDto product, string stripeProductId, string stripePriceId)
        {
            var productService = new ProductService(_client);
            var priceService = new PriceService(_client);

            var updatedProduct = await productService.UpdateAsync(stripeProductId, new ProductUpdateOptions
            {
                Name = product.Name,
                Description = product.Description,
            });

            await productService.UpdateAsync(stripePriceId, new ProductUpdateOptions
            {
                Active = false
            });

            var recurring = CreatePriceRecurringOptions(product.Interval);

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
                Recurring = recurring
            });

            return new(createdPrice.Id, createdProduct.Id);
        }

        public async Task DeleteProductStripeAsync(string priceId, string productId)
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

        private PriceRecurringOptions? CreatePriceRecurringOptions(IntervalEnum paymentTypeEnum)
        {
            PriceRecurringOptions recurring = null;

            switch (paymentTypeEnum)
            {
                case IntervalEnum.Mountly:
                    recurring = new PriceRecurringOptions
                    {
                        Interval = "month",
                        IntervalCount = 1,
                    };
                    break;
                case IntervalEnum.Yearly:
                    recurring = new PriceRecurringOptions
                    {
                        Interval = "year",
                        IntervalCount = 1,
                    };
                    break;
                case IntervalEnum.OneTime:
                    break;
                default:
                    throw new Exception("Unknown paymentTypeEnum: " + paymentTypeEnum.ToString());
            }

            return recurring;
        }
    }
}
