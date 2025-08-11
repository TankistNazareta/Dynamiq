using Docker.DotNet.Models;
using Dynamiq.Application.Commands.Products.Commands;
using Dynamiq.Application.DTOs;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Domain.Enums;
using Dynamiq.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Net.Http.Json;

namespace Dynamiq.API.Tests.Integrations.Products
{
    public class UpdateProductTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public UpdateProductTests(CustomWebApplicationFactory<Program> factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
            _factory = factory;
        }

        [Fact]
        public async Task UpdateProduct_WithValidData_ShouldReturnOk()
        {
            await ProductServiceForTests.CreateProductAsync(_factory, _scopeFactory);

            var stripeServiceMock = new Mock<IStripeProductService>();
            stripeServiceMock
                .Setup(s => s.UpdateProductStripeAsync(It.IsAny<ProductDto>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new StripeIdsDto("stripe_price_123", "stripe_product_123"));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IStripeProductService));
                    if (descriptor != null)
                        services.Remove(descriptor);
                    services.AddSingleton(stripeServiceMock.Object);
                });
            }).CreateClient();

            Guid productId;
            Guid categoryId;

            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IProductRepo>();
                var products = await repo.GetAllAsync(CancellationToken.None);
                var created = products.Should().ContainSingle(p => p.Name == "Test Product").Subject;

                productId = created.Id;
                categoryId = created.CategoryId;
            }

            var command = new UpdateProductCommand(productId, "NewName", "NewDescr", 11111, IntervalEnum.OneTime, categoryId);

            var response = await client.PutAsJsonAsync("/product", command);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IProductRepo>();
                var products = await repo.GetAllAsync(CancellationToken.None);
                var created = products.Should().ContainSingle(p => p.Name == "NewName").Subject;

                created.Should().NotBeNull();
                created.Description.Should().Be("NewDescr");
                created.Price.Should().Be(11111);
            }

            stripeServiceMock.Verify(s => s.UpdateProductStripeAsync(
                It.IsAny<ProductDto>(), 
                It.IsAny<string>(), 
                It.IsAny<string>()
            ), Times.Once);
        }
    }
}
