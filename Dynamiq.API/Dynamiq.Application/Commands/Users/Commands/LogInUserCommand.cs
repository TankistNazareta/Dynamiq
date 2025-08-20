using Dynamiq.Application.DTOs.AuthDTOs;
using MediatR;

namespace Dynamiq.Application.Commands.Users.Commands
{
    public record class LogInUserCommand(string Email, string Password) : IRequest<AuthTokensDto>;
}
