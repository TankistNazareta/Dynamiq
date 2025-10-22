using Dynamiq.Domain.Enums;
using MediatR;

namespace Dynamiq.Application.Commands.Subscriptions.Commands
{
    public record class UpdateSubscriptionCommand(Guid Id, string Name, int Price, IntervalEnum Interval) : IRequest;
}
