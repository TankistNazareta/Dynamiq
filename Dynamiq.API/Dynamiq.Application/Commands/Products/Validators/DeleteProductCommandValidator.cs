using Dynamiq.Application.Commands.Products.Commands;
using FluentValidation;

namespace Dynamiq.Application.Commands.Products.Validators
{
    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Product Id is required.");
        }
    }
}
