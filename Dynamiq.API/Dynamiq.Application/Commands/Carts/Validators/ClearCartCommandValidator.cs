using Dynamiq.Application.Commands.Carts.Commands;
using FluentValidation;

namespace Dynamiq.Application.Commands.Carts.Validators
{
    public class ClearCartCommandValidator : AbstractValidator<ClearCartCommand>
    {
        public ClearCartCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");
        }
    }
}
