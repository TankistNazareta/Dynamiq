using Dynamiq.API.Tests.Integrations.Products;
using Dynamiq.API.Tests.Integrations.Users;
using Dynamiq.Application.Commands.Carts.Commands;
using Dynamiq.Application.DTOs.AccountDTOs;
using Dynamiq.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

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
            Guid userId;
            Guid productId;
            var productName = "Test Product for RemoveItemFromCart " + Guid.NewGuid();

            var email = $"testEmail{Guid.NewGuid()}";

            await UserServiceForTests.CreateUserAndConfirmHisEmail(_factory, _client, new(email, "sdlfkjsdfds"));
            await ProductServiceForTests.CreateProductAsync(_factory, _factory.Services.GetRequiredService<IServiceScopeFactory>(), productName);

            using(var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetService<AppDbContext>();
                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
                userId = user!.Id;
                var product = await db.Products.FirstOrDefaultAsync(p => p.Name == productName);
                productId = product!.Id;
            }

            var addResponse = await _client.PutAsync(
                $"/cart?userId={userId}&productId={productId}&quantity=3",
                new StringContent("")
            );

            addResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var response = await _client.PutAsync($"/cart?userId={userId}&productId={productId}&quantity=1", new StringContent(""));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var cart = await response.Content.ReadFromJsonAsync<CartDto>();
            cart.Should().NotBeNull();
            cart!.Items.Should().HaveCount(1);
            cart.Items.First().Quantity.Should().Be(1);
        }
    }
}
