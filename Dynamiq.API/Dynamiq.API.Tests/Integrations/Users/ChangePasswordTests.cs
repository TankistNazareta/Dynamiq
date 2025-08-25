using Dynamiq.API.Tests.ResponseDtos;
using Dynamiq.Application.Commands.Users.Commands;
using Dynamiq.Application.DTOs.CommonDTOs;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Dynamiq.API.Tests.Integrations.Users
{
    public class ChangePasswordTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private HttpClient _client;

        public ChangePasswordTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private async Task<HttpResponseMessage> MakeChangePasswordRequest(ChangeUserPasswordCommand command)
        {
            var signUpCommand = new RegisterUserCommand(command.Email, "OldPassword123!");
            await UserServiceForTests.CreateuserAndConfirmHisEmail(_factory, _client, signUpCommand);

            var logInCommand = new LogInUserCommand(command.Email, "OldPassword123!");

            var logInResponse = await _client.PostAsJsonAsync("/auth/log-in", logInCommand);

            var logInResponseDto = await logInResponse.Content.ReadFromJsonAsync<LogInResponse>();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", logInResponseDto.AccessToken);

            var response = await _client.PutAsJsonAsync("/user/change-password", command);

            return response;
        }

        [Fact]

        public async Task ChangePassword_WithValidData_ShouldReturnOk()
        {
            var command = new ChangeUserPasswordCommand($"newEmail{Guid.NewGuid():N}@gmail.com", "OldPassword123!", "NewPassword456!");

            var result = await MakeChangePasswordRequest(command);
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ChangePassword_WithWrongOldPassword_ShouldReturnBadRequest()
        {
            var command = new ChangeUserPasswordCommand($"email{Guid.NewGuid():N}@email.com", "WrongPassword123!", "NewPassword456!");

            var result = await MakeChangePasswordRequest(command);

            var response = await result.Content.ReadFromJsonAsync<ExceptionResponseDto>();
            response.Should().BeEquivalentTo(new ExceptionResponseDto(
                HttpStatusCode.BadRequest,
                "your old password isn't correct"
            ));
        }
    }
}
