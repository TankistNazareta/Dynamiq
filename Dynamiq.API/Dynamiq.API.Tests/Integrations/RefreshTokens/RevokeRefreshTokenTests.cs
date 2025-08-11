using Dynamiq.Application.Commands.RefreshTokens.Commands;
using Dynamiq.Application.DTOs;
using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Enums;
using Dynamiq.Infrastructure.Persistence.Context;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Net;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Dynamiq.API.Tests.Integrations.RefreshTokens
{
    public class RevokeRefreshTokenTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public RevokeRefreshTokenTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RevokeToken_Should_ReturnOk_And_UpdateUser()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var user = new User(
                email: "test@example.com",
                passwordHash: "hashed_password",
                role: RoleEnum.DefaultUser);

            var oldRefreshToken = "old-refresh-token";
            user.AddRefreshToken(new RefreshToken(user.Id, oldRefreshToken));

            db.Users.Add(user);
            await db.SaveChangesAsync();

            var command = new RevokeRefreshTokenCommand(oldRefreshToken);

            var response = await _client.PutAsJsonAsync("/token/revoke", command);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadAsStringAsync();
            result.Should().Be("Token successfully revoked");

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
