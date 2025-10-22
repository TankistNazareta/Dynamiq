using Dynamiq.Application.Commands.Carts.Commands;
using FluentValidation;

namespace Dynamiq.Application.Commands.Carts.Validators
{
    public class SetQuantityCartItemCommandValidator : AbstractValidator<SetQuantityCartItemCommand>
    {
        public SetQuantityCartItemCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty();

            RuleFor(x => x.Quantity)
                .GreaterThan(0);
        }
    }
}