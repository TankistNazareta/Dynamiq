using Dynamiq.API.Tests.ResponseDtos;
using Dynamiq.Application.Commands.Users.Commands;
using Dynamiq.Application.Interfaces.Auth;
using Dynamiq.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Dynamiq.API.Tests.Integrations.Users
{
    public class DeleteUserTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private HttpClient _client;

        public DeleteUserTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task DeleteUser_WithValidData_ShouldReturnOk()
        {
            var email = $"test{Guid.NewGuid():N}@exampleee.com";

            var signUpCommand = new RegisterUserCommand(email, "OldPassword123!");
            await UserServiceForTests.CreateuserAndConfirmHisEmail(_factory, _client, signUpCommand);

            var logInCommand = new LogInUserCommand(email, "OldPassword123!");
            var authResponse = await _client.PostAsJsonAsync("/auth/log-in", logInCommand);

            var LogInResponseDto = await authResponse.Content.ReadFromJsonAsync<LogInResponse>();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LogInResponseDto.AccessToken);

            using var scope = _factory.Services.CreateScope();
            var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var user = await db.Users.FirstOrDefaultAsync(x => x.Email == email);

            var response = await _client.DeleteAsync($"/users/{user.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
