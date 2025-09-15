using Dynamiq.Application.Commands.Products.Commands;
using FluentValidation;

namespace Dynamiq.Application.Commands.Products.Validators
{
    public class AddViewCountCommandValidator : AbstractValidator<AddViewCountCommand>
    {
        public AddViewCountCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Product Id is required.");
        }
    }
}
