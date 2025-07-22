using MediatR;

namespace Dynamiq.Application.Commands.Users.Commands
{
    public record DeleteUserCommand(Guid Id) : IRequest;
}
