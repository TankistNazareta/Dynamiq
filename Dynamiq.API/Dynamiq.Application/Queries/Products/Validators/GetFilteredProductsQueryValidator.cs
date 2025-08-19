using Dynamiq.Application.Queries.Products.Queries;
using FluentValidation;

namespace Dynamiq.Application.Queries.Products.Validators
{
    public class GetFilteredProductsQueryValidator : AbstractValidator<GetFilteredProductsQuery>
    {
        public GetFilteredProductsQueryValidator()
        {
            RuleFor(x => x.Limit)
                .GreaterThan(0).WithMessage("Limit must be greater than 0")
                .LessThanOrEqualTo(100).WithMessage("Limit cannot be greater than 100");

            RuleFor(x => x.Offset)
                .GreaterThanOrEqualTo(0).WithMessage("Offset cannot be negative");

            RuleFor(x => x.Filter)
                .NotNull().WithMessage("Filter cannot be null")
                .ChildRules(filter =>
                {
                    filter.RuleFor(f => f.MinPrice)
                        .GreaterThanOrEqualTo(0)
                        .When(f => f.MinPrice.HasValue)
                        .WithMessage("MinPrice cannot be negative");

                    filter.RuleFor(f => f.MaxPrice)
                        .GreaterThanOrEqualTo(0)
                        .When(f => f.MaxPrice.HasValue)
                        .WithMessage("MaxPrice cannot be negative");

                    filter.RuleFor(f => f)
                        .Must(f => !f.MinPrice.HasValue || !f.MaxPrice.HasValue || f.MinPrice <= f.MaxPrice)
                        .WithMessage("MinPrice cannot be greater than MaxPrice");

                    filter.RuleFor(f => f.SearchTerm)
                        .MaximumLength(100)
                        .When(f => !string.IsNullOrWhiteSpace(f.SearchTerm))
                        .WithMessage("SearchTerm cannot exceed 100 characters");
                });
        }
    }
}
