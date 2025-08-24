using Dynamiq.Application.Queries.Categories.Queries;
using FluentValidation;

namespace Dynamiq.Application.Queries.Categories.Validators
{
    public class GetCategoryWithParentByIdQueryValidator : AbstractValidator<GetCategoryWithParentByIdQuery>
    {
        public GetCategoryWithParentByIdQueryValidator()
        {
            RuleFor(x => x.ChildId)
                .NotEmpty().WithMessage("ChildId cannot be empty");
        }
    }
}
