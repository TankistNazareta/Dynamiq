using Dynamiq.Application.Commands.Subscriptions.Commands;
using Dynamiq.Domain.Enums;
using FluentValidation;

namespace Dynamiq.Application.Commands.Subscriptions.Validators
{
    public class UpdateSubscriptionCommandValidator : AbstractValidator<UpdateSubscriptionCommand>
    {
        public UpdateSubscriptionCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Subscription ID is required.");

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
