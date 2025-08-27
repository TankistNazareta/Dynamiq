using Dynamiq.Domain.Common;
using Dynamiq.Domain.Entities;

namespace Dynamiq.Domain.Events
{
    public class CartItemAddedEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; private set; }
        public CartItem CartItem { get; private set; }

        public CartItemAddedEvent(CartItem cartItem)
        {
            OccurredOn = DateTime.UtcNow;
            CartItem = cartItem;
        }
    }
}
