using Dynamiq.Application.DTOs.AuthDTOs;
using Dynamiq.Application.Interfaces.Auth;
using Dynamiq.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite.Storage.Json.Internal;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace Dynamiq.API.Tests.Integrations.GoogleAuth
{
    public class GoogleCallbackTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        private string _email;

        public GoogleCallbackTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Замінюємо IGoogleOidcService на мок
                    var oidcDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IGoogleOidcService));
                    if (oidcDescriptor != null) services.Remove(oidcDescriptor);

                    var mockGoogle = new Mock<IGoogleOidcService>();
                    mockGoogle.Setup(x => x.ExchangeCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                              .ReturnsAsync(new GoogleTokenResponse { id_token = "test-id-token" });
                    mockGoogle.Setup(x => x.ValidateIdTokenAsync("test-id-token", It.IsAny<CancellationToken>()))
                              .ReturnsAsync((new ClaimsPrincipal(new ClaimsIdentity(new[]
                              {
                              new Claim(JwtRegisteredClaimNames.Sub, "123"),
                              new Claim(JwtRegisteredClaimNames.Email, GeneretaNewEmail()),
                              new Claim("name", "Test User")
                              }, "oidc")), null));

                    services.AddSingleton(mockGoogle.Object);
                });
            }).CreateClient();
        }

        private string GeneretaNewEmail()
        {
            _email = $"test{Guid.NewGuid():N}@example.com";
            return _email;
        }

        [Fact]
        public async Task Callback_WhenUserDoesNotExist_ReturnsAuthResponse()
        {
            var query = new Dictionary<string, string>
            {
                ["code"] = "test-code",
                ["state"] = "teststate"
            };
            var url = QueryHelpers.AddQueryString("/auth/google/callback", query);

            var response = await _client.GetAsync(url);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetService<AppDbContext>();

                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == _email);
                user.Should().NotBeNull();
                user.Email.Should().Be(_email);
                user.PasswordHash.Should().Be(IPasswordService.DefaultHashForOidc);
            }
        }
    }
}
