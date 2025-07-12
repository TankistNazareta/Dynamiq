using Dynamiq.API.Mapping.DTOs;
using FluentValidation;

namespace Dynamiq.API.Validation.DTOsValidator
{
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(u => u.PasswordHash)
                .NotEmpty().WithMessage("PasswordHash is required.");

            RuleFor(u => u.Role)
                .IsInEnum().WithMessage("Invalid Role.");

            When(u => u.RefreshToken != null, () =>
            {
                RuleFor(u => u.RefreshToken).SetValidator(new RefreshTokenDtoValidator());
            });

            RuleForEach(u => u.PaymentHistories).SetValidator(new PaymentHistoryDtoValidator());

            RuleForEach(u => u.Subscriptions).SetValidator(new SubscriptionDtoValidator());

            When(u => u.EmailVerification != null, () =>
            {
                RuleFor(u => u.EmailVerification).SetValidator(new EmailVerificationDtoValidator());
            });
        }
    }
}
