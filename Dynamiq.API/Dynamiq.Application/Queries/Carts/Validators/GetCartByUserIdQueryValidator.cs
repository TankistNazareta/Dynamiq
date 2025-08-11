using Dynamiq.Application.Queries.Products.Queries;
using FluentValidation;

namespace Dynamiq.Application.Queries.Carts.Validators
{
    public class GetCartByUserIdQueryValidator : AbstractValidator<GetProductByIdQuery>
    {
        public GetCartByUserIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User Id is required.");
        }
    }
}
