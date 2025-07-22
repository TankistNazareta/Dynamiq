using Dynamiq.Application.Commands.Payment.Commands;
using FluentValidation;

namespace Dynamiq.Application.Commands.Payment.Validators
{
    public class StripeWebhookCommandValidator : AbstractValidator<StripeWebhookCommand>
    {
        public StripeWebhookCommandValidator()
        {
            RuleFor(x => x.Json)
                .NotEmpty().WithMessage("Webhook JSON payload is required.");

            RuleFor(x => x.Signature)
                .NotEmpty().WithMessage("Stripe signature is required.")
                .MaximumLength(500).WithMessage("Signature must not exceed 500 characters.");
        }
    }
}
