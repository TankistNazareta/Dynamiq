using Dynamiq.API.Extension.RequestEntity;
using FluentValidation;

namespace Dynamiq.API.Validation.RequestsEntitiesValidation
{
    public class ProductRequestEntityValidator : AbstractValidator<ProductRequestEntity>
    {
        public ProductRequestEntityValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(x => x.PaymentType)
                .IsInEnum().WithMessage("PaymentType must be a valid enum value.");
        }
    }
}
