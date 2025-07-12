using Dynamiq.API.Mapping.DTOs;
using FluentValidation;

namespace Dynamiq.API.Validation.DTOsValidator
{
    public class PaymentHistoryDtoValidator : AbstractValidator<PaymentHistoryDto>
    {
        public PaymentHistoryDtoValidator()
        {
            RuleFor(ph => ph.StripePaymentId)
                .NotEmpty().WithMessage("StripePaymentId is required.");

            RuleFor(ph => ph.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");

            RuleFor(ph => ph.Interval)
                .IsInEnum().WithMessage("Invalid Interval.");

            RuleFor(ph => ph.CreatedAt)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("CreatedAt cannot be in the future.");

            RuleFor(ph => ph.UserId).NotEmpty().WithMessage("UserId is required.");
            RuleFor(ph => ph.ProductId).NotEmpty().WithMessage("ProductId is required.");
        }
    }
}
