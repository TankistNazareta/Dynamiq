using Dynamiq.Application.Commands.Users.Commands;
using Dynamiq.Application.DTOs.CommonDTOs;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace Dynamiq.API.Tests.Integrations.Users
{
    public class RegisterUserTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public RegisterUserTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        private RegisterUserCommand CreateValidCommand(string email) =>
            new RegisterUserCommand(email, "StrongP@ssword1");

        private async Task<HttpResponseMessage> RegisterAsync(RegisterUserCommand command) =>
            await _client.PostAsJsonAsync("/auth/signup", command);

        [Fact]
        public async Task RegisterUser_WithValidData_ShouldReturnSuccess()
        {
            var command = CreateValidCommand($"test{Guid.NewGuid():N}@example.com");
            var response = await RegisterAsync(command);

            Console.WriteLine(response);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();

            content.Should().Contain("You successfully registered");
        }

        [Fact]
        public async Task RegisterUser_WithDuplicateEmail_ShouldReturnError()
        {
            var command = CreateValidCommand("duplicate@example.com");
            await RegisterAsync(command);

            var secondResponseJson = await RegisterAsync(command);

            var exceptionResult = await secondResponseJson.Content.ReadFromJsonAsync<ExceptionResponseDto>();

            exceptionResult.Should().BeEquivalentTo(
                new ExceptionResponseDto(
                    HttpStatusCode.BadRequest,
                    "User with this email is already exist"
                ));
        }

        [Fact]
        public async Task RegisterUser_WithInvalidEmail_ShouldReturnValidationError()
        {
            var command = CreateValidCommand("invalid-email");
            var response = await RegisterAsync(command);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var error = await response.Content.ReadAsStringAsync();

            error.Should().Contain("Email must be a valid email address");
        }

        [Fact]
        public async Task RegisterUser_WithShortPassword_ShouldReturnValidationError()
        {
            var command = new RegisterUserCommand("shortpass@example.com", "123");
            var response = await RegisterAsync(command);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var error = await response.Content.ReadAsStringAsync();

            error.Should().ContainAny("Password must be at least 6 characters.");
        }

        [Fact]
        public async Task RegisterUser_WithEmptyBody_ShouldReturnBadRequest()
        {
            var response = await RegisterAsync(new(null!, null!));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
