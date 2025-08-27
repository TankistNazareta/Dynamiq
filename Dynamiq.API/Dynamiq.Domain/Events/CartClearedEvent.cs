using Dynamiq.Domain.Common;

namespace Dynamiq.Domain.Events
{
    public class CartClearedEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; private set; }
        public Guid CartId { get; private set; }
        public CartClearedEvent(Guid cartId)
        {
            OccurredOn = DateTime.UtcNow;
            CartId = cartId;
        }
    }
}
