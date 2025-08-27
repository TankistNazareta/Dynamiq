using Dynamiq.API.Tests.Integrations.Products;
using Dynamiq.API.Tests.Integrations.Users;
using Dynamiq.Application.Commands.Carts.Commands;
using Dynamiq.Application.DTOs.AccountDTOs;
using Dynamiq.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
            Guid userId;
            Guid productId;
            var productName = "Test Product for AddItemToCart_NoCart " + Guid.NewGuid();
            var email = $"testEmail{Guid.NewGuid()}";

            await UserServiceForTests.CreateUserAndConfirmHisEmail(_factory, _client, new(email, "password123"));
            await ProductServiceForTests.CreateProductAsync(_factory, _factory.Services.GetRequiredService<IServiceScopeFactory>(), productName);

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                userId = (await db.Users.FirstOrDefaultAsync(u => u.Email == email))!.Id;
                productId = (await db.Products.FirstOrDefaultAsync(p => p.Name == productName))!.Id;
            }

            var response = await _client.PostAsync($"/cart?userId={userId}&productId={productId}&quantity=2", new StringContent(""));

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<CartDto>();

            result.Should().NotBeNull();
            result!.Items.Should().HaveCount(1);
            result.Items[0].ProductId.Should().Be(productId);
            result.Items[0].Quantity.Should().Be(2);
        }

        [Fact]
        public async Task AddItemToCart_WhenCartExists_ShouldIncreaseQuantityOfExistingItem()
        {
            Guid userId;
            Guid productId;
            var productName = "Test Product for AddItemToCart_ExistingCart " + Guid.NewGuid();
            var email = $"testEmail{Guid.NewGuid()}";

            await UserServiceForTests.CreateUserAndConfirmHisEmail(_factory, _client, new(email, "password123"));
            await ProductServiceForTests.CreateProductAsync(_factory, _factory.Services.GetRequiredService<IServiceScopeFactory>(), productName);

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                userId = (await db.Users.FirstOrDefaultAsync(u => u.Email == email))!.Id;
                productId = (await db.Products.FirstOrDefaultAsync(p => p.Name == productName))!.Id;
            }
            
            await _client.PostAsJsonAsync($"/cart?userId={userId}&productId={productId}&quantity=1", new StringContent(""));

            var response = await _client.PostAsync($"/cart?userId={userId}&productId={productId}&quantity=3", new StringContent(""));

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<CartDto>();

            result.Should().NotBeNull();
            result!.Items.Should().ContainSingle();
            result.Items[0].ProductId.Should().Be(productId);
            result.Items[0].Quantity.Should().Be(4);
        }
    }
}
