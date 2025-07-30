using MediatR;

namespace Dynamiq.Application.Commands.Payment.Commands
{
    public record class CreateCheckoutSessionCommand(
        Guid UserId,
        string SuccessUrl, string CancelUrl,
        Guid? CartId,
        int? Quantity, Guid? ProductId
        ) : IRequest<string>;
}
