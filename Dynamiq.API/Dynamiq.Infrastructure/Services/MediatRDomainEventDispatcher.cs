using Dynamiq.Domain.Common;
using Dynamiq.Domain.Interfaces;
using MediatR;

namespace Dynamiq.Infrastructure.Services
{
    public class MediatRDomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IMediator _mediator;

        public MediatRDomainEventDispatcher(IMediator mediator)
            => _mediator = mediator;

        public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents)
        {
            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent);
            }
        }
    }
}
