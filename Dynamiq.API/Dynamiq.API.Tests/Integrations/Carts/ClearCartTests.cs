using Dynamiq.API.Tests.Integrations.Products;
using Dynamiq.API.Tests.Integrations.Users;
using Dynamiq.Application.Commands.Carts.Commands;
using Dynamiq.Application.Commands.Users.Commands;
using Dynamiq.Domain.Aggregates;
using Dynamiq.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Dynamiq.API.Tests.Integrations.Carts
{
    [Collection("CartTests")]
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
            Guid userId;
            Guid productId;
            var productName = "Test Product for ClearCart " + Guid.NewGuid();
            var email = $"ClearCartUserEmail{Guid.NewGuid():N}@test.com";

            await UserServiceForTests.CreateUserAndConfirmHisEmail(_factory, _client, new(email, "password123"));
            await ProductServiceForTests.CreateProductAsync(_factory, _factory.Services.GetRequiredService<IServiceScopeFactory>(), productName);

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                userId = (await db.Users.FirstAsync(u => u.Email == email)).Id;
                productId = (await db.Products.FirstAsync(p => p.Name == productName)).Id;
            }

            TestAuthHandler.TestUserId = userId;

            var addResponse = await _client.PostAsync($"/cart?productId={productId}&quantity=2", new StringContent(""));
            addResponse.EnsureSuccessStatusCode();

            var response = await _client.DeleteAsync($"/cart");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseGet = await _client.GetAsync($"/cart");
            responseGet.StatusCode.Should().Be(HttpStatusCode.NotFound);

            TestAuthHandler.TestUserId = null;
        }


        [Fact]
        public async Task ClearCart_WhenCartDoesNotExist_ShouldReturnNotFound()
        {
            var email = $"ClearCartUserEmailReturnNotFound{Guid.NewGuid():N}@test.com";

            var commandRegisterUser = new RegisterUserCommand(email, "hashPAss");
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await UserServiceForTests.CreateUserAndConfirmHisEmail(_factory, _client, commandRegisterUser);

            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

            TestAuthHandler.TestUserId = user.Id;

            var clearCommand = new ClearCartCommand();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri("/cart", UriKind.Relative),
                Content = JsonContent.Create(clearCommand)
            };

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            TestAuthHandler.TestUserId = null;
        }
    }
}
