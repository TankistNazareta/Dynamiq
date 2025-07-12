using Dynamiq.Auth.DTOs;
using FluentValidation;

namespace Dynamiq.Auth.Validations
{
    public class AuthUserDtoValidator : AbstractValidator<AuthUserDto>
    {
        public AuthUserDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }

}
