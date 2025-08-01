using MediatR;

namespace Dynamiq.Application.Commands.Payment.Commands
{
    public record class StripeWebhookCommand(string Json, string Signature) : IRequest<bool>;
}
