namespace Dynamiq.Domain.ValueObject
{
    public class CartItem
    {
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }

        private CartItem() { }

        public CartItem(Guid productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        public void IncreaseQuantity(int amount)
        {
            Quantity += amount;
        }

        public void SetQuantity(int quantity)
        {
            Quantity = quantity;
        }
    }
}
