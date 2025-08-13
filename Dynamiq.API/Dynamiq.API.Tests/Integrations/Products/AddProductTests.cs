using Dynamiq.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Dynamiq.API.Tests.Integrations.Products
{
    public class AddProductTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public AddProductTests(CustomWebApplicationFactory<Program> factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
            _factory = factory;
        }

        [Fact]
        public async Task AddProduct_WithValidData_ShouldReturnOk()
        {
            string productName = $"{Guid.NewGuid():N} Product";

            await ProductServiceForTests.CreateProductAsync(_factory, _scopeFactory, productName);

            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IProductRepo>();
                var products = await repo.GetAllAsync(CancellationToken.None);
                var created = products.Should().ContainSingle(p => p.Name == productName).Subject;

                created.StripeProductId.Should().Be("stripe_product_123");
                created.StripePriceId.Should().Be("stripe_price_123");
            }
        }
    }
}
