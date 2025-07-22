using MediatR;

namespace Dynamiq.Application.Commands.EmailVerifications.Commands
{
    public record class ConfirmEmailByTokenCommand(string Token) : IRequest;
}
