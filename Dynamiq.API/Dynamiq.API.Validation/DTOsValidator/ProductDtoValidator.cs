using Dynamiq.API.Mapping.DTOs;
using FluentValidation;

namespace Dynamiq.API.Validation.DTOsValidator
{
    public class ProductDtoValidator : AbstractValidator<ProductDto>
    {
        public ProductDtoValidator()
        {
            RuleFor(p => p.StripeProductId)
                .NotEmpty().WithMessage("StripeProductId is required.");

            RuleFor(p => p.StripePriceId)
                .NotEmpty().WithMessage("StripePriceId is required.");

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Name is required.");

            RuleFor(p => p.Description)
                .NotEmpty().WithMessage("Description is required.");

            RuleFor(p => p.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(p => p.Interval)
                .IsInEnum().WithMessage("Invalid Interval.");

            RuleForEach(p => p.PaymentHistories).SetValidator(new PaymentHistoryDtoValidator());
        }
    }
}
