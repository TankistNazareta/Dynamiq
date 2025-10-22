using Dynamiq.Application.Commands.Subscriptions.Command;
using Dynamiq.Domain.Enums;
using FluentValidation;

namespace Dynamiq.Application.Commands.Subscriptions.Validator
{
    public class AddSubscriptionCommandValidator : AbstractValidator<AddSubscriptionCommand>
    {
        public AddSubscriptionCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Subscription name is required.")
                .MinimumLength(3).WithMessage("Subscription name must be at least 3 characters long.")
                .MaximumLength(100).WithMessage("Subscription name must not exceed 100 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(x => x.Interval)
                .Must(v => Enum.IsDefined(typeof(IntervalEnum), v))
                .WithMessage("Invalid subscription interval.");
        }
    }

}
