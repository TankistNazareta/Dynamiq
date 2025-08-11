using Docker.DotNet.Models;
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
            var signUpCommand = new RegisterUserCommand("email@test.com", "OldPassword123!");
            await UserServiceForTests.CreateuserAndConfirmHisEmail(_factory, _client, signUpCommand);

            var logInCommand = new LogInUserCommand("email@test.com", "OldPassword123!");

            var authResponse = await _client.PostAsJsonAsync("/auth/login", logInCommand);

            var authResult = await authResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var user = await db.Users.Include(x => x.EmailVerification).FirstOrDefaultAsync(x => x.EmailVerification.IsConfirmed);

            var response = await _client.DeleteAsync($"/users/{user.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().Be("user was removed");
        }
    }
}
