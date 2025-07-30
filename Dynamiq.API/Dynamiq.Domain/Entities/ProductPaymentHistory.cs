using Dynamiq.Domain.Enums;

namespace Dynamiq.Domain.Entities
{
    public class ProductPaymentHistory
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public Guid PaymentHistoryId { get; set; }
        public int Quantity { get; private set; }

        private ProductPaymentHistory() { } // EF Core

        public ProductPaymentHistory(Guid productId, Guid paymentHistoryId, int quantity = 1)
        {
            if (quantity <= 0)
                throw new ArgumentException("quantity must be greater than zero", nameof(quantity));

            Id = Guid.NewGuid();
            ProductId = productId;
            Quantity = quantity;
            PaymentHistoryId = paymentHistoryId;
        }
    }
}
