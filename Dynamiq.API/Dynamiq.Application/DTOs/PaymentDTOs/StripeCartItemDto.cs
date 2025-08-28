namespace Dynamiq.Application.DTOs.PaymentDTOs
{
    public class StripeCartItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string StripePriceId { get; set; }
        public int Price { get; set; }

        public StripeCartItemDto(Guid productId, int quantity, string stripePriceId, int price)
        {
            ProductId = productId;
            Quantity = quantity;
            StripePriceId = stripePriceId;
            Price = price;
        }
    }
}
