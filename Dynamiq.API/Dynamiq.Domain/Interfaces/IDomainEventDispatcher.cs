using Dynamiq.Domain.Common;

namespace Dynamiq.Domain.Interfaces
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents);
    }
}
