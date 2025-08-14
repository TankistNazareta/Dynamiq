using Dynamiq.Application.Commands.Carts.Commands;
using System.Net.Http.Json;
using System.Net;
using FluentAssertions;
using Dynamiq.Application.DTOs.AccountDTOs;

namespace Dynamiq.API.Tests.Integrations.Carts
{
    public class RemoveItemFromCartTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private HttpClient _client;

        public RemoveItemFromCartTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RemoveItemFromCart_WithValidData_ShouldReturnUpdatedCart()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var addCommand = new AddItemToCartCommand(userId, productId, 3);
            var addResponse = await _client.PostAsJsonAsync($"/cart", addCommand);
            addResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var removeCommand = new RemoveItemFromCartCommand(userId, productId, 1);

            var response = await _client.PutAsJsonAsync("/cart", removeCommand);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var cart = await response.Content.ReadFromJsonAsync<CartDto>();
            cart.Should().NotBeNull();
            cart!.Items.Should().HaveCount(1);
            cart.Items.First().Quantity.Should().Be(2);
        }

        [Fact]
        public async Task RemoveItemFromCart_WithNonExistingCart_ShouldReturnNotFound()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var removeCommand = new RemoveItemFromCartCommand(userId, productId, 1);

            var response = await _client.PutAsJsonAsync("/cart", removeCommand);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
