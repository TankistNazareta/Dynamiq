using Dynamiq.Application.Commands.Subscriptions.Commands;
using FluentValidation;

namespace Dynamiq.Application.Commands.Subscriptions.Validators
{
    public class DeleteSubscriptionCommandValidator : AbstractValidator<DeleteSubscriptionCommand>
    {
        public DeleteSubscriptionCommandValidator()
        {
            RuleFor(x => x.SubscriptionId)
                .NotEmpty().WithMessage("Subscription ID is required.");
        }
    }
}
