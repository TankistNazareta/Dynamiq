using Dynamiq.Domain.ValueObject;

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
            Id = Guid.NewGuid();
            UserId = userId;
        }

        public void AddItem(Guid productId, int quantity)
        {
            var existing = _items.FirstOrDefault(i => i.ProductId == productId);
            if (existing != null)
                existing.IncreaseQuantity(quantity);
            else
                _items.Add(new CartItem(productId, quantity));
        }

        public void RemoveItem(Guid productId, int quantity)
        {
            var item = _items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null) return;

            if (item.Quantity > quantity)
                item.SetQuantity(item.Quantity - quantity);
            else
                _items.Remove(item);
        }

        public void Clear() => _items.Clear();
    }
}
