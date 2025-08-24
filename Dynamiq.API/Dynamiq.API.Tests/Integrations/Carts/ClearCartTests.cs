using Dynamiq.API.Tests.Integrations.Users;
using Dynamiq.Application.Commands.Carts.Commands;
using Dynamiq.Application.Commands.Users.Commands;
using Dynamiq.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Dynamiq.API.Tests.Integrations.Carts
{
    public class ClearCartTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ClearCartTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task ClearCart_WhenCartExists_ShouldRemoveCart()
        {
            var email = $"ClearCartUserEmail{Guid.NewGuid():N}@test.com";

            var commandRegisterUser = new RegisterUserCommand(email, "hashPAss");
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await UserServiceForTests.CreateuserAndConfirmHisEmail(_factory, _client, commandRegisterUser);

            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

            var productId = Guid.NewGuid();

            var addCommand = new AddItemToCartCommand(user.Id, productId, 2);

            var resp = await _client.PostAsJsonAsync("/cart", addCommand);

            var clearCommand = new ClearCartCommand(user.Id);
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
                Content = JsonContent.Create(user.Id)
            };

            var responseGet = await _client.SendAsync(requestGet);

            responseGet.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ClearCart_WhenCartDoesNotExist_ShouldReturnNotFound()
        {
            var email = $"ClearCartUserEmailReturnNotFound{Guid.NewGuid():N}@test.com";

            var commandRegisterUser = new RegisterUserCommand(email, "hashPAss");
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await UserServiceForTests.CreateuserAndConfirmHisEmail(_factory, _client, commandRegisterUser);

            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

            var clearCommand = new ClearCartCommand(user.Id);
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
