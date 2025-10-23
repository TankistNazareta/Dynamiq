using Dynamiq.Application.Commands.Products.Commands;
using Dynamiq.Application.DTOs.ProductDTOs;
using Dynamiq.Application.DTOs.PaymentDTOs;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Domain.Enums;
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
            string productName = $"{Guid.NewGuid():N} Product";

            await ProductServiceForTests.CreateProductAsync(_factory, _scopeFactory, productName);

            var stripeServiceMock = new Mock<IStripeProductService>();
            stripeServiceMock
                .Setup(s => s.UpdateProductStripeAsync(It.IsAny<ProductDto>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IntervalEnum?>()))
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
                var response = await repo.GetAllAsync(100, 0, CancellationToken.None);
                var created = response.Products.Should().ContainSingle(p => p.Name == productName).Subject;

                productId = created.Id;
                categoryId = created.CategoryId;
            }

            var newName = $"newName{Guid.NewGuid():N}";

            var command = new UpdateProductCommand(
                Id: productId,
                Name: newName,
                Description: "NewDescr",
                Price: 11111,
                CategoryId: categoryId,
                ImgUrls: new List<string> { "https://example.com/image.jpg" },
                Paragraphs: new List<string> { "Updated paragraph 1", "Updated paragraph 2" },
                CardDescription: "Updated card description"
            );

            var responseHttp = await client.PutAsJsonAsync("/product", command);
            responseHttp.StatusCode.Should().Be(HttpStatusCode.OK);

            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IProductRepo>();
                var response = await repo.GetAllAsync(100, 0, CancellationToken.None);
                var updated = response.Products.Should().ContainSingle(p => p.Name == newName).Subject;

                updated.Should().NotBeNull();
                updated.Description.Should().Be("NewDescr");
                updated.Price.Should().Be(11111);
            }

            stripeServiceMock.Verify(s => s.UpdateProductStripeAsync(
                It.IsAny<ProductDto>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<IntervalEnum?>()
            ), Times.Once);
        }
    }
}
