using MediatR;

namespace Dynamiq.Application.Commands.Users.Commands
{
    public record class ChangeUserPasswordCommand(string Email, string OldPassword, string NewPassword) : IRequest;
}
