using Dynamiq.Domain.Common;
using Dynamiq.Domain.Entities;

namespace Dynamiq.Domain.Events
{
    public class CartItemClearEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; private set; }
        public CartItem CartItem { get; private set; }

        public CartItemClearEvent(CartItem cartItem)
        {
            CartItem = cartItem;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
