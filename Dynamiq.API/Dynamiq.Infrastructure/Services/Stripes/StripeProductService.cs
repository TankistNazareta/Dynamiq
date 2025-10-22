using Dynamiq.Application.DTOs.ProductDTOs;
using Dynamiq.Application.DTOs.PaymentDTOs;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Dynamiq.Infrastructure.Services.Stripes
{
    public class StripeProductService : IStripeProductService
    {
        private readonly StripeClient _client;

        public StripeProductService(IConfiguration config)
        {
            var secretKey = Environment.GetEnvironmentVariable("STRIPE_SECRET")!
                ?? throw new ArgumentNullException("Stripe:SecretKey is not configured");
            _client = new StripeClient(secretKey);
        }

        public async Task<StripeIdsDto> CreateProductStripeAsync(ProductDto product, IntervalEnum? intervalEnum = null)
        {
            var productService = new ProductService(_client);
            var priceService = new PriceService(_client);

            var recurring = CreatePriceRecurringOptions(intervalEnum);

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

        public async Task<StripeIdsDto> UpdateProductStripeAsync(ProductDto product, string stripeProductId, string stripePriceId, IntervalEnum? intervalEnum = null)
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

            var recurring = CreatePriceRecurringOptions(intervalEnum);

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

        private PriceRecurringOptions? CreatePriceRecurringOptions(IntervalEnum? paymentTypeEnum)
        {
            PriceRecurringOptions recurring = null;

            if(paymentTypeEnum == null)
                return recurring;

            switch (paymentTypeEnum)
            {
                case IntervalEnum.Monthly:
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
                default:
                    throw new Exception("Unknown paymentTypeEnum: " + paymentTypeEnum.ToString());
            }

            return recurring;
        }
    }
}
