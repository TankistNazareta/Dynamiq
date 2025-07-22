using Dynamiq.Application.Commands.RefreshTokens.Commands;
using FluentValidation;

namespace Dynamiq.Application.Commands.RefreshTokens.Validators
{
    public class RevokeRefreshTokenCommandValidator : AbstractValidator<RevokeRefreshTokenCommand>
    {
        public RevokeRefreshTokenCommandValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required.")
                .MaximumLength(500).WithMessage("Token must not exceed 500 characters.");
        }
    }

}
