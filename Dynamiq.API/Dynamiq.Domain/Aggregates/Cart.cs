using Dynamiq.Domain.Common;
using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Events;

namespace Dynamiq.Domain.Aggregates
{
    public class Cart : BaseEntity
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        private readonly List<CartItem> _items = new();

        public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

        private Cart() { }

        public Cart(Guid userId)
        {
            UserId = userId;

        }

        public void AddItem(Guid productId, int quantity)
        {
            var existing = _items.FirstOrDefault(i => i.ProductId == productId);
            if (existing != null)
                existing.IncreaseQuantity(quantity);
            else
            {
                var cartItem = new CartItem(productId, quantity, Id);

                _items.Add(cartItem);
                AddDomainEvent(new CartItemAddedEvent(cartItem));
            }
        }

        public void RemoveItem(Guid productId, int quantity)
        {
            var item = _items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null) return;

            if (item.Quantity > quantity)
            {
                item.SetQuantity(item.Quantity - quantity);
                AddDomainEvent(new CartItemRemovedEvent(item));
            }
            else
            {
                _items.Remove(item);
                AddDomainEvent(new CartItemClearEvent(item));
            }
        }

        public void Clear()
        {
            _items.Clear();
            AddDomainEvent(new CartClearedEvent(Id));
        }
    }
}
