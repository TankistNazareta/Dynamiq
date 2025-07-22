using Dynamiq.Application.Queries.Users.Queries;
using FluentValidation;

namespace Dynamiq.Application.Queries.Users.Validators
{
    public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
    {
        public GetUserByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User Id is required.");
        }
    }
}
