using Dynamiq.API.Mapping.DTOs;
using FluentValidation;

namespace Dynamiq.API.Validation.DTOsValidator
{
    public class SubscriptionDtoValidator : AbstractValidator<SubscriptionDto>
    {
        public SubscriptionDtoValidator()
        {
            RuleFor(s => s.StartDate)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("StartDate cannot be in the future.");

            RuleFor(s => s.EndDate)
                .GreaterThan(s => s.StartDate).WithMessage("EndDate must be after StartDate.");

            RuleFor(s => s.ProductId).NotEmpty().WithMessage("ProductId is required.");
            RuleFor(s => s.UserId).NotEmpty().WithMessage("UserId is required.");

            When(s => s.Product != null, () =>
            {
                RuleFor(s => s.Product).SetValidator(new ProductDtoValidator());
            });
        }
    }
}
