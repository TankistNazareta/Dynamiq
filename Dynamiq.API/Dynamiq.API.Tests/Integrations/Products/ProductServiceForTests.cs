using Dynamiq.Application.Commands.Products.Commands;
using Dynamiq.Application.DTOs.ProductDTOs;
using Dynamiq.Application.DTOs.StripeDTOs;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Enums;
using Dynamiq.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Net.Http.Json;

namespace Dynamiq.API.Tests.Integrations.Products
{
    public static class ProductServiceForTests
    {
        public static async Task CreateProductAsync(CustomWebApplicationFactory<Program> factory, IServiceScopeFactory scopeFactory, string productName)
        {
            var stripeServiceMock = new Mock<IStripeProductService>();
            stripeServiceMock
                .Setup(s => s.CreateProductStripeAsync(It.IsAny<ProductDto>()))
                .ReturnsAsync(new StripeIdsDto("stripe_price_123", "stripe_product_123"));

            var client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IStripeProductService));
                    if (descriptor != null)
                        services.Remove(descriptor);
                    services.AddSingleton(stripeServiceMock.Object);
                });
            }).CreateClient();

            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var category = new Category($"Test Category For {Guid.NewGuid():N}");
                db.Categories.Add(category);
                await db.SaveChangesAsync();
            }

            Guid categoryId;
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                categoryId = db.Categories.First().Id;
            }

            var command = new AddProductCommand(
                productName,
                "Test Description",
                1000,
                IntervalEnum.OneTime,
                categoryId
            );

            var response = await client.PostAsJsonAsync("/product", command);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            stripeServiceMock.Verify(s => s.CreateProductStripeAsync(It.IsAny<ProductDto>()), Times.Once);
        }
    }
}
