using Dynamiq.Domain.Common;
using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Events;

namespace Dynamiq.Domain.Aggregates
{
    public class Cart
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

        public void SetItemQuantity(Guid productId, int quantity)
        {
            var item = _items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
            {
                item = new CartItem(productId, quantity, Id);
                _items.Add(item);
            }
            else
            {
                if (quantity == 0 || item.Quantity == 0)
                    _items.Remove(item);
                else
                    item.SetQuantity(quantity);
            }
        }

        public void AddItem(Guid productId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

            var item = _items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
            {
                item = new CartItem(productId, quantity, Id);
                _items.Add(item);
            }
            else
            {
                item.SetQuantity(item.Quantity + quantity);
            }
        }

        public void Clear()
        {
            _items.Clear();
        }
    }
}
