using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Enums;
using Dynamiq.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Dynamiq.API.Tests.Integrations.RefreshTokens
{
    public class RevokeRefreshTokenTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public RevokeRefreshTokenTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                HandleCookies = true
            });
        }

        [Fact]
        public async Task RevokeToken_Should_ReturnOk_And_UpdateUser()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var user = new User(
                email: $"test{Guid.NewGuid():N}@example.com",
                passwordHash: "hashed_password",
                role: RoleEnum.DefaultUser);

            var oldRefreshToken = $"old-refresh-token-{Guid.NewGuid():N}";
            user.AddRefreshToken(new RefreshToken(user.Id, oldRefreshToken));

            db.Users.Add(user);
            await db.SaveChangesAsync();

            _client.DefaultRequestHeaders.Add("Cookie", $"refreshToken={oldRefreshToken}");

            var response = await _client.PutAsync("/token/revoke", null);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            db.ChangeTracker.Clear();

            var updatedUser = await db.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == oldRefreshToken));
            updatedUser.Should().NotBeNull();

            var firstTokenId = user.RefreshTokens.First().Id;

            updatedUser.RefreshTokens
                .FirstOrDefault(rt => rt.Id == firstTokenId)
                .Should()
                .Match<RefreshToken>(rt => !rt.IsActive());
        }
    }
}
