using MediatR;

namespace Dynamiq.Application.Commands.Users.Commands
{
    public record RegisterUserCommand(string Email, string Password) : IRequest;
}
