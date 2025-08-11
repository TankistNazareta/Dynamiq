using Dynamiq.Application.Commands.Carts.Commands;
using Dynamiq.Application.DTOs;
using FluentAssertions;
using System.Net.Http.Json;

namespace Dynamiq.API.Tests.Integrations.Carts
{
    public class AddItemToCartTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private HttpClient _client;

        public AddItemToCartTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task AddItemToCart_WhenCartDoesNotExist_ShouldCreateNewCartAndAddItem()
        {
            var command = new AddItemToCartCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                2
            );

            var response = await _client.PostAsJsonAsync("/cart", command);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<CartDto>();

            result.Should().NotBeNull();
            result!.Items.Should().HaveCount(1);
            result.Items[0].ProductId.Should().Be(command.ProductId);
            result.Items[0].Quantity.Should().Be(2);
        }

        [Fact]
        public async Task AddItemToCart_WhenCartExists_ShouldIncreaseQuantityOfExistingItem()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var firstAdd = new AddItemToCartCommand(userId, productId, 1);

            await _client.PostAsJsonAsync("/cart", firstAdd);

            var secondAdd = new AddItemToCartCommand(userId, productId, 3);

            var response = await _client.PostAsJsonAsync("/cart", secondAdd);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<CartDto>();

            result.Should().NotBeNull();
            result!.Items.Should().ContainSingle();
            result.Items[0].ProductId.Should().Be(productId);
            result.Items[0].Quantity.Should().Be(4);
        }
    }
}
