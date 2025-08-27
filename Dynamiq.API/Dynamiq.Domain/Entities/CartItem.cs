namespace Dynamiq.Domain.Entities
{
    public class CartItem
    {
        public Guid Id { get; private set; }
        public Guid CartId { get; private set; }
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }

        private CartItem() { }

        public CartItem(Guid productId, int quantity, Guid cartId)
        {
            ProductId = productId;
            Quantity = quantity;
            CartId = cartId;
        }

        public void IncreaseQuantity(int amount) => Quantity += amount;
        public void SetQuantity(int newQuantity) => Quantity = newQuantity;
    }
}
