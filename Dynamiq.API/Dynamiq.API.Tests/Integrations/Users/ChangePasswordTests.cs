using Dynamiq.Application.Commands.Users.Commands;
using Dynamiq.Application.DTOs;
using Dynamiq.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Dynamiq.API.Tests.Integrations.Users
{
    public class ChangePasswordTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private const string _defaultEmail = "email@test.com";

        private readonly CustomWebApplicationFactory<Program> _factory;
        private HttpClient _client;

        public ChangePasswordTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private async Task<HttpResponseMessage> MakeChangePasswordRequest(ChangeUserPasswordCommand command)
        {
            var signUpCommand = new RegisterUserCommand(_defaultEmail, "OldPassword123!");
            await UserServiceForTests.CreateuserAndConfirmHisEmail(_factory, _client, signUpCommand);

            var logInCommand = new LogInUserCommand(_defaultEmail, "OldPassword123!");

            var authResponse = await _client.PostAsJsonAsync("/auth/login", logInCommand);

            var authResult = await authResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);

            var response = await _client.PutAsJsonAsync("/users/change-password", command);

            return response;
        }

        [Fact]

        public async Task ChangePassword_WithValidData_ShouldReturnOk()
        {
            var command = new ChangeUserPasswordCommand(_defaultEmail, "OldPassword123!", "NewPassword456!");

            var result = await MakeChangePasswordRequest(command);
            result.StatusCode.Should().Be(HttpStatusCode.OK);

            var response = await result.Content.ReadAsStringAsync();
            response.Should().Be("You successfully changed your password");
        }

        [Fact]
        public async Task ChangePassword_WithWrongOldPassword_ShouldReturnBadRequest()
        {
            var command = new ChangeUserPasswordCommand(_defaultEmail, "WrongPassword123!", "NewPassword456!");

            var result = await MakeChangePasswordRequest(command);

            var response = await result.Content.ReadFromJsonAsync<ExceptionResponseDto>();
            response.Should().BeEquivalentTo(new ExceptionResponseDto(
                HttpStatusCode.BadRequest, 
                "your old password isn't correct"
            ));
        }
    }
}
