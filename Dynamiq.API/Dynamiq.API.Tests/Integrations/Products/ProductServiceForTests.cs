using Dynamiq.Application.Commands.Products.Commands;
using Dynamiq.Application.DTOs;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Enums;
using Dynamiq.Domain.Interfaces.Repositories;
using Dynamiq.Infrastructure.Persistence.Context;
using Moq;
using System.Net.Http.Json;
using System.Net;
using Docker.DotNet.Models;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Dynamiq.API.Tests.Integrations.Products
{
    public static class ProductServiceForTests
    {
        public static async Task CreateProductAsync(CustomWebApplicationFactory<Program> factory, IServiceScopeFactory scopeFactory)
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
                var category = new Category("TestCategory");
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
                "Test Product",
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
