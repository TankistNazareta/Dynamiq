using Dynamiq.Application.Commands.Carts.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamiq.Application.Commands.Carts.Validators
{
    public class RemoveItemFromCartCommandValidator : AbstractValidator<RemoveItemFromCartCommand>
    {
        public RemoveItemFromCartCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        }
    }
}
