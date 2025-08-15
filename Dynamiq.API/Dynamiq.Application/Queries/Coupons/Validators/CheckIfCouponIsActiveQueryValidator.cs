using Dynamiq.Application.Queries.Coupons.Queries;
using FluentValidation;

namespace Dynamiq.Application.Queries.Coupons.Validators
{
    public class CheckIfCouponIsActiveQueryValidator : AbstractValidator<CheckIfCouponIsActiveQuery>
    {
        public CheckIfCouponIsActiveQueryValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Coupon code is required.")
                .MaximumLength(50).WithMessage("Coupon code must not exceed 50 characters.");
        }
    }
}
