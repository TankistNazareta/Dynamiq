using Dynamiq.API.Mapping.DTOs;
using FluentValidation;

namespace Dynamiq.API.Validation.DTOsValidator
{
    public class RefreshTokenDtoValidator : AbstractValidator<RefreshTokenDto>
    {
        public RefreshTokenDtoValidator()
        {
            RuleFor(r => r.Token)
                .NotEmpty().WithMessage("Token is required.");

            RuleFor(r => r.ExpiresAt)
                .GreaterThan(DateTime.UtcNow).WithMessage("ExpiresAt must be in the future.");

            RuleFor(r => r.UserId).NotEmpty().WithMessage("UserId is required.");
        }
    }
}
