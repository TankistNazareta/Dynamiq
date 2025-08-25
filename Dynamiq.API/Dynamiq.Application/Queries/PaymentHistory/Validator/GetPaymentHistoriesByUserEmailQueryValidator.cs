using Dynamiq.Application.Queries.PaymentHistory.Queries;
using FluentValidation;

namespace Dynamiq.Application.Queries.PaymentHistory.Validator
{
    public class GetPaymentHistoriesByUserEmailQueryValidator : AbstractValidator<GetPaymentHistoriesByUserEmailQuery>
    {
        public GetPaymentHistoriesByUserEmailQueryValidator()
        {
            RuleFor(x => x.UserEmail)
                .NotEmpty().WithMessage("Users Id is required.");
        }
    }
}
