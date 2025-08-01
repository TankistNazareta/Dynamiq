using MediatR;

namespace Dynamiq.Application.Commands.Payment.Commands
{
    public record class CreateCheckoutSessionCommand(
        Guid UserId,
        string SuccessUrl, string CancelUrl,
        int? Quantity, Guid? ProductId
        ) : IRequest<string>;
}
