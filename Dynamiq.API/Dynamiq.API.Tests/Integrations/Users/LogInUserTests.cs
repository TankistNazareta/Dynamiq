using Dynamiq.Application.Commands.Users.Commands;
using Dynamiq.Application.DTOs.CommonDTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Json;

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
            await _client.PostAsJsonAsync("/auth/log-in", command);

        [Fact]
        public async Task LoginUser_WithValidCredentials_ShouldReturnSuccessAndToken()
        {
            var email = $"login.success{Guid.NewGuid():N}@example.com";
            var password = "StrongP@ssword1";
            await UserServiceForTests.CreateuserAndConfirmHisEmail(_factory, _client, new RegisterUserCommand(email, password));

            var response = await LoginAsync(new(email, password));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var accessToken = await response.Content.ReadAsStringAsync();

            accessToken.Should().NotBeNull();
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
