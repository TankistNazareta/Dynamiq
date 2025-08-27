using Dynamiq.API.Tests.Integrations.Users;
using Dynamiq.Application.Commands.EmailVerifications.Commands;
using Dynamiq.Application.Commands.Users.Commands;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Dynamiq.API.Tests.Integrations.EmailVerifications
{
    public class ConfirmEmailByTokenTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ConfirmEmailByTokenTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ConfirmEmailByToken_WithValidToken_ShouldConfirmEmail()
        {
            var registerCommand = new RegisterUserCommand(
                $"user_{Guid.NewGuid():N}@test.com",
                "StrongPass123!"
            );

            await UserServiceForTests.CreateUserAndConfirmHisEmail(_factory, _client, registerCommand);
            using var scope = _factory.Services.CreateScope();

            var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepo>();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var user = await db.Users.Include(x => x.EmailVerification).FirstOrDefaultAsync(x => x.EmailVerification.IsConfirmed);

            user!.EmailVerification.IsConfirmed.Should().BeTrue();
        }

        [Fact]
        public async Task ConfirmEmailByToken_WithInvalidToken_ShouldReturnNotFound()
        {
            var confirmCommand = new ConfirmEmailByTokenCommand("invalid-token");
            var response = await _client.PostAsJsonAsync("/email/confirm", confirmCommand);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
