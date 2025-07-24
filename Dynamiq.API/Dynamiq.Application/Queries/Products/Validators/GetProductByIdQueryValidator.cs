using Dynamiq.Application.Queries.Products.Queries;
using FluentValidation;

namespace Dynamiq.Application.Queries.Products.Validators
{
    public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
    {
        public GetProductByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User Id is required.");
        }
    }

}
