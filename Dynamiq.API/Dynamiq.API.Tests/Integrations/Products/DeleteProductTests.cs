using Dynamiq.Application.DTOs;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;

namespace Dynamiq.API.Tests.Integrations.Products
{
    public class DeleteProductTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public DeleteProductTests(CustomWebApplicationFactory<Program> factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task DeleteProduct_WithValidData_ShouldReturnOk()
        {
            string productName = $"{Guid.NewGuid():N} Product";

            await ProductServiceForTests.CreateProductAsync(_factory, _scopeFactory, productName);

            var stripeServiceMock = new Mock<IStripeProductService>();
            stripeServiceMock
                .Setup(s => s.DeleteProductStripeAsync(It.IsAny<string>(), It.IsAny<string>()));

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

            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IProductRepo>();
                var products = await repo.GetAllAsync(100, 0, CancellationToken.None);
                var created = products.Should().ContainSingle(p => p.Name == productName).Subject;

                productId = created.Id;
            }

            var res = await client.DeleteAsync($"/product?id={productId}");
            res.StatusCode.Should().Be(HttpStatusCode.OK);

            stripeServiceMock.Verify(s => s.DeleteProductStripeAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
