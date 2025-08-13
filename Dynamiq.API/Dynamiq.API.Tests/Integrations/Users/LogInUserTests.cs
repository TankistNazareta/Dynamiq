using Dynamiq.Application.Commands.Users.Commands;
using FluentAssertions;
using System.Net.Http.Json;
using System.Net;
using Dynamiq.Infrastructure.Persistence.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Dynamiq.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace Dynamiq.API.Tests.Integrations.Users
{
    public class LoginUserTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private HttpClient _client;

        public LoginUserTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private async Task<HttpResponseMessage> LoginAsync(LogInUserCommand command) =>
            await _client.PostAsJsonAsync("/auth/login", command);

        [Fact]
        public async Task LoginUser_WithValidCredentials_ShouldReturnSuccessAndToken()
        {
            var email = $"login.success{Guid.NewGuid():N}@example.com";
            var password = "StrongP@ssword1";
            await UserServiceForTests.CreateuserAndConfirmHisEmail(_factory, _client, new RegisterUserCommand(email, password));

            var response = await LoginAsync(new(email, password));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var res = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

            res.AccessToken.Should().NotBeNull();
            res.RefreshToken.Should().NotBeNull();
        }

        [Fact]
        public async Task LoginUser_WithInvalidPassword_ShouldReturnUnauthorized()
        {
            var email = $"login.invalidpass{Guid.NewGuid():N}@example.com";
            var password = "StrongP@ssword1";
            await UserServiceForTests.CreateuserAndConfirmHisEmail(_factory, _client, new RegisterUserCommand(email, password));

            var response = await LoginAsync(new(email, "WrongPassword!"));

            var exceptionResult = await response.Content.ReadFromJsonAsync<ExceptionResponseDto>();

            exceptionResult.Should().BeEquivalentTo(
                new ExceptionResponseDto(
                    HttpStatusCode.BadRequest,
                    "Your password isn't correct"
                ));
        }

        [Fact]
        public async Task LoginUser_WithNonExistingEmail_ShouldReturnUnauthorized()
        {
            var response = await LoginAsync(new("notfound@example.com", "SomePass123!"));

            var exceptionResult = await response.Content.ReadFromJsonAsync<ExceptionResponseDto>();

            exceptionResult.Should().BeEquivalentTo(
                new ExceptionResponseDto(
                    HttpStatusCode.NotFound,
                    "User with this email wasn't found"
                ));
        }
    }
}
