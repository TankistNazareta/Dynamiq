using Dynamiq.Application.Queries.Products.Queries;
using FluentValidation;

namespace Dynamiq.Application.Queries.Products.Validators
{
    internal class GetAllProductBySlugQueryValidator : AbstractValidator<GetAllProductBySlugQuery>
    {
        public GetAllProductBySlugQueryValidator()
        {
            RuleFor(x => x.Slug)
                .NotEmpty().WithMessage("Slug is required.");
        }
    }
}
