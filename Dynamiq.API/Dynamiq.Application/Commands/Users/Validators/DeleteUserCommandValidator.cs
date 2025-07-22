using Dynamiq.Application.Commands.Users.Commands;
using FluentValidation;

namespace Dynamiq.Application.Commands.Users.Validators
{
    public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User Id is required.");
        }
    }
}
