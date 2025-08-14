using Dynamiq.Application.Commands.RefreshTokens.Commands;
using Dynamiq.Application.DTOs.AuthDTOs;
using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Enums;
using Dynamiq.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Dynamiq.API.Tests.Integrations.RefreshTokens
{
    public class RefreshTheTokenTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public RefreshTheTokenTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RefreshToken_Should_Return_New_AuthResponse_And_Update_User()
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

            var command = new RefreshTheTokenCommand(oldRefreshToken);

            var response = await _client.PutAsJsonAsync("/token/refresh", command);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            result.AccessToken.Should().NotBeNull();
            result.RefreshToken.Should().NotBeNull();
            result.RefreshToken.Should().NotBeEquivalentTo(oldRefreshToken);

            var updatedUser = await db.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == result.RefreshToken));
            updatedUser.Should().NotBeNull();
        }
    }
}
