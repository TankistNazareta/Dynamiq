using Dynamiq.Application.Commands.Users.Commands;
using FluentValidation;

namespace Dynamiq.Application.Commands.Users.Validators
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.")
                .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
                .MaximumLength(64).WithMessage("Password must not exceed 64 characters.");
        }
    }
}
