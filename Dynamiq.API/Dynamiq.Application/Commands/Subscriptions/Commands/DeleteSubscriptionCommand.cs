using MediatR;

namespace Dynamiq.Application.Commands.Subscriptions.Commands
{
    public record class DeleteSubscriptionCommand(Guid SubscriptionId) : IRequest;
}
