using Dynamiq.API.Mapping.DTOs;
using FluentValidation;

namespace Dynamiq.API.Validation.DTOsValidator
{
    public class EmailVerificationDtoValidator : AbstractValidator<EmailVerificationDto>
    {
        public EmailVerificationDtoValidator()
        {
            RuleFor(ev => ev.Token)
                .NotEmpty().WithMessage("Token is required.");

            RuleFor(ev => ev.ExpiresAt)
                .GreaterThan(DateTime.UtcNow).WithMessage("ExpiresAt must be in the future.");

            RuleFor(ev => ev.UserId).NotEmpty().WithMessage("UserId is required.");
        }
    }
}
