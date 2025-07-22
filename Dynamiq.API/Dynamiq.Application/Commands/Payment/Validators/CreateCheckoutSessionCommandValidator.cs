using Dynamiq.Application.Commands.Payment.Commands;
using FluentValidation;

namespace Dynamiq.Application.Commands.Payment.Validators
{
    public class CreateCheckoutSessionCommandValidator : AbstractValidator<CreateCheckoutSessionCommand>
    {
        public CreateCheckoutSessionCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId is required.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.SuccessUrl)
                .NotEmpty().WithMessage("SuccessUrl is required.")
                .MaximumLength(500).WithMessage("SuccessUrl must not exceed 500 characters.")
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .WithMessage("SuccessUrl must be a valid URL.");

            RuleFor(x => x.CancelUrl)
                .NotEmpty().WithMessage("CancelUrl is required.")
                .MaximumLength(500).WithMessage("CancelUrl must not exceed 500 characters.")
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .WithMessage("CancelUrl must be a valid URL.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        }
    }
}
