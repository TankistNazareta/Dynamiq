using Dynamiq.Application.Queries.Products.Queries;
using FluentValidation;

namespace Dynamiq.Application.Queries.Products.Validators
{
    public class GetSearchedProductQueryValidator : AbstractValidator<GetSearchedProductQuery>
    {
        public GetSearchedProductQueryValidator()
        {
            RuleFor(x => x.ProductFilter)
                .NotNull().WithMessage("ProductFilter is required.");

            RuleFor(x => x.ProductFilter.SearchTerm)
                .NotEmpty().WithMessage("SearchTerm is required.");

            RuleFor(x => x.Limit)
                .GreaterThan(0).WithMessage("Limit must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("Limit cannot exceed 100.");
        }
    }
}
