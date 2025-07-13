using MediatR;

namespace Dynamiq.API.Commands.EmailVerification
{
    public record ConfirmEmailCommand(Guid Id) : IRequest;
}
