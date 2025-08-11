using Dynamiq.Application.Commands.Users.Commands;
using Dynamiq.Application.DTOs;
using Dynamiq.Domain.Entities;
using Dynamiq.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Dynamiq.API.Tests.Integrations.Users
{
    public static class UserServiceForTests
    {
        public static async Task CreateuserAndConfirmHisEmail(
            CustomWebApplicationFactory<Program> factory,
            HttpClient client,
            RegisterUserCommand command)
        {
            await client.PostAsJsonAsync("/auth/signup", command);

            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var emailVerification = await db.EmailVerifications.FirstOrDefaultAsync(ev => !ev.IsConfirmed);

            if(emailVerification != null)
                await client.PutAsync($"/emailVerification/confirm?token={emailVerification.Token}", null);
        }
    }
}
