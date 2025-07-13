using Dynamiq.API.Extension.RequestEntity;
using FluentValidation;

namespace Dynamiq.API.Validation.RequestsEntitiesValidation
{
    public class CheckoutSessionRequestValidator : AbstractValidator<CheckoutSessionRequest>
    {
        public CheckoutSessionRequestValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId is required.");

            RuleFor(x => x.StripePriceId)
                .NotEmpty().WithMessage("StripePriceId is required.");

            RuleFor(x => x.SuccessUrl)
                .NotEmpty().WithMessage("SuccessUrl is required.")
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("SuccessUrl must be a valid URL.");

            RuleFor(x => x.CancelUrl)
                .NotEmpty().WithMessage("CancelUrl is required.")
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("CancelUrl must be a valid URL.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");

            RuleFor(x => x.PaymentTypeEnum)
                .IsInEnum().WithMessage("PaymentTypeEnum is not valid.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");
        }
    }
}
