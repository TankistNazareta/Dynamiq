using Dynamiq.Application.Commands.GoogleAuth.Commands;
using FluentValidation;

namespace Dynamiq.Application.Commands.GoogleAuth.Validators
{
    public class GoogleCallbackCommandValidator : AbstractValidator<GoogleCallbackCommand>
    {
        public GoogleCallbackCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Authorization code is required.")
                .MaximumLength(2048).WithMessage("Authorization code is too long.");

            RuleFor(x => x.State)
                .MaximumLength(256).WithMessage("State value is too long.")
                .When(x => !string.IsNullOrEmpty(x.State));
        }
    }
}
