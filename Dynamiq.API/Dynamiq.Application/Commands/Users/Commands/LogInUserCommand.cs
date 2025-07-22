using Dynamiq.Application.DTOs;
using MediatR;

namespace Dynamiq.Application.Commands.Users.Commands
{
    public record class LogInUserCommand(string Email, string Password) : IRequest<AuthResponseDto>;
}
