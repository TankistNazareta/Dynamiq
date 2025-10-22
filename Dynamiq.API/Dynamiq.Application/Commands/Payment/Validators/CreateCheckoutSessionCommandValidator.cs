using Dynamiq.Application.Commands.Payment.Commands;
using FluentValidation;

namespace Dynamiq.Application.Commands.Payment.Validators
{
    public class CreateCheckoutSessionCommandValidator : AbstractValidator<CreateCheckoutSessionCommand>
    {
        public CreateCheckoutSessionCommandValidator()
        {
            RuleFor(x => x.SuccessUrl)
                .NotEmpty().WithMessage("SuccessUrl is required")
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("SuccessUrl must be a valid URL");

            RuleFor(x => x.CancelUrl)
                .NotEmpty().WithMessage("CancelUrl is required")
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("CancelUrl must be a valid URL");

            When(x => x.ProductId.HasValue, () =>
            {
                RuleFor(x => x.Quantity)
                    .NotNull().WithMessage("Quantity is required when ProductId is specified")
                    .GreaterThan(0).WithMessage("Quantity must be greater than zero");
            });
        }
    }
}
