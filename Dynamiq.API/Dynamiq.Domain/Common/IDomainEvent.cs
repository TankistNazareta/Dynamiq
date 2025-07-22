using MediatR;

namespace Dynamiq.Domain.Common
{
    public interface IDomainEvent : INotification
    {
        DateTime OccurredOn { get; }
    }
}
