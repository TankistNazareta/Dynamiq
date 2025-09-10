using Dynamiq.Application.Commands.Carts.Commands;
using FluentValidation;

namespace Dynamiq.Application.Commands.Carts.Validators
{
    public class AddCartItemCommandValidator : AbstractValidator<AddCartItemCommand>
    {
        public AddCartItemCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.ProductId)
                .NotEmpty();

            RuleFor(x => x.Quantity)
                .GreaterThan(0);
        }
    }
}
