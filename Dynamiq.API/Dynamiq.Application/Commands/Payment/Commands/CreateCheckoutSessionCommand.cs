using MediatR;

namespace Dynamiq.Application.Commands.Payment.Commands
{
    public record class CreateCheckoutSessionCommand(
        Guid ProductId, Guid UserId,
        string SuccessUrl, string CancelUrl,
        int Quantity
        ) : IRequest<string>;
}
