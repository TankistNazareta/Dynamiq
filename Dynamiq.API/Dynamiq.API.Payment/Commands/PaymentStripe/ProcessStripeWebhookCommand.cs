using Dynamiq.API.Mapping.DTOs;
using MediatR;

namespace Dynamiq.API.Stripe.Commands.PaymentStripe
{
    public record ProcessStripeWebhookCommand(string Json, string Signature) : IRequest<PaymentHistoryDto>;
}
