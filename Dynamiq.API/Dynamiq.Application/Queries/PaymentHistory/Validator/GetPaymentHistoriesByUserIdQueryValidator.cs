using Dynamiq.Application.Queries.PaymentHistory.Queries;
using FluentValidation;

namespace Dynamiq.Application.Queries.PaymentHistory.Validator
{
    public class GetPaymentHistoriesByUserIdQueryValidator : AbstractValidator<GetPaymentHistoriesByUserIdQuery>
    {
        public GetPaymentHistoriesByUserIdQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Payment history id is required.");
        }
    }
}
