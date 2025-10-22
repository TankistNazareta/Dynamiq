using Dynamiq.Domain.Enums;
using MediatR;

namespace Dynamiq.Application.Commands.Subscriptions.Command
{
    public record class AddSubscriptionCommand(string Name, int Price, IntervalEnum Interval) : IRequest;
}
