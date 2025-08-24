using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Enums;
using Dynamiq.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Dynamiq.API.Tests.Integrations.RefreshTokens
{
    public class RefreshTheTokenTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public RefreshTheTokenTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
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

            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                HandleCookies = true
            });

            client.DefaultRequestHeaders.Add("Cookie", $"refreshToken={oldRefreshToken}");

            var response = await client.PutAsync("/token/refresh", null);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadAsStringAsync();
            result.Should().NotBeNull();

            var setCookieHeaders = response.Headers
                .Where(h => h.Key.Equals("Set-Cookie", StringComparison.OrdinalIgnoreCase))
                .SelectMany(h => h.Value);
            var refreshToken = setCookieHeaders
                .Select(h => h.Split(';')[0])
                .Select(s => s.Split('='))
                .Where(parts => parts.Length == 2 && parts[0].Trim() == "refreshToken")
                .Select(parts => WebUtility.UrlDecode(parts[1]))
                .FirstOrDefault();

            refreshToken.Should().NotBeNull();

            using var newScope = _factory.Services.CreateScope();
            var newDb = newScope.ServiceProvider.GetRequiredService<AppDbContext>();

            var updatedRt = newDb.RefreshTokens
                .FirstOrDefault(rt => rt.Token == refreshToken);

            updatedRt.Should().NotBeNull();
            updatedRt.UserId.Should().Be(user.Id);
        }
    }
}
