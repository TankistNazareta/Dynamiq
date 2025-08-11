using Dynamiq.Application.Commands.Carts.Commands;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace Dynamiq.API.Tests.Integrations.Carts
{
    public class ClearCartTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ClearCartTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ClearCart_WhenCartExists_ShouldRemoveCart()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var addCommand = new AddItemToCartCommand(userId, productId, 2);

            await _client.PostAsJsonAsync("/cart", addCommand);

            var clearCommand = new ClearCartCommand(userId);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri("/cart", UriKind.Relative),
                Content = JsonContent.Create(clearCommand)
            };

            var response = await _client.SendAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var requestGet = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("/cart", UriKind.Relative),
                Content = JsonContent.Create(userId)
            }; 
            
            var responseGet = await _client.SendAsync(requestGet);

            responseGet.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ClearCart_WhenCartDoesNotExist_ShouldReturnNotFound()
        {
            var clearCommand = new ClearCartCommand(Guid.NewGuid());
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri("/cart", UriKind.Relative),
                Content = JsonContent.Create(clearCommand)
            };

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
