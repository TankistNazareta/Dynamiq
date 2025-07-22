using Dynamiq.Application.Commands.Products.Commands;
using FluentValidation;

namespace Dynamiq.Application.Commands.Products.Validators
{
    internal class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Product Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.Interval)
                .IsInEnum().WithMessage("Interval must be a valid enum value.");
        }
    }
}
