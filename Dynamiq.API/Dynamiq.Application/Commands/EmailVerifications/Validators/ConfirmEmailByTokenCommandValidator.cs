using Dynamiq.Application.Commands.EmailVerifications.Commands;
using FluentValidation;

namespace Dynamiq.Application.Commands.EmailVerifications.Validators
{
    public class ConfirmEmailByTokenCommandValidator : AbstractValidator<ConfirmEmailByTokenCommand>
    {
        public ConfirmEmailByTokenCommandValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required.")
                .MaximumLength(200).WithMessage("Token must not exceed 200 characters.");
        }
    }
}
