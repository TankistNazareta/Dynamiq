using Dynamiq.Application.Commands.Products.Commands;
using FluentValidation;

namespace Dynamiq.Application.Commands.Products.Validators
{
    public class AddProductCommandValidator : AbstractValidator<AddProductCommand>
    {
        public AddProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.Interval)
                .IsInEnum().WithMessage("Interval must be a valid enum value.");

            RuleFor(x => x.ImgUrls)
                .NotEmpty().WithMessage("At least one image URL is required.")
                .ForEach(urlRule => urlRule
                    .Must(IsValidUrl)
                    .WithMessage("Each image URL must be a valid URL."));
        }

        private bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
