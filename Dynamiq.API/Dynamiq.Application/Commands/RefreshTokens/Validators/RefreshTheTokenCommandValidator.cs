using Dynamiq.Application.Commands.RefreshTokens.Commands;
using FluentValidation;

namespace Dynamiq.Application.Commands.RefreshTokens.Validators
{
    public class RefreshTheTokenCommandValidator : AbstractValidator<RefreshTheTokenCommand>
    {
        public RefreshTheTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Token is required.")
                .MaximumLength(500).WithMessage("Token must not exceed 500 characters.");
        }
    }
}
