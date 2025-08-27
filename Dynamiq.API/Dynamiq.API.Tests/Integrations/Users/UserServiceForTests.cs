using Dynamiq.Application.Commands.Users.Commands;
using Dynamiq.Application.Interfaces.Auth;
using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Enums;
using Dynamiq.Infrastructure.Persistence.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Dynamiq.API.Tests.Integrations.Users
{
    public static class UserServiceForTests
    {
        public static async Task CreateUserAndConfirmHisEmail(
            CustomWebApplicationFactory<Program> factory,
            HttpClient client,
            RegisterUserCommand command)
        {
            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();

            var user = new User(command.Email, passwordService.HashPassword(command.Password), RoleEnum.DefaultUser);
            db.Users.Add(user);

            await db.SaveChangesAsync();

            var emailVerification = new EmailVerification(user.Id);
            emailVerification.Confirm(command.Email);
            db.EmailVerifications.Add(emailVerification);

            await db.SaveChangesAsync();
        }
    }
}
