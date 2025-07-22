using Dynamiq.Domain.Common;
using Dynamiq.Domain.Enums;

namespace Dynamiq.Domain.Entities
{
    public class PaymentHistory : BaseEntity
    {
        public Guid Id { get; private set; }
        public string StripePaymentId { get; private set; }
        public decimal Amount { get; private set; }
        public IntervalEnum Interval { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Guid UserId { get; private set; }
        public Guid ProductId { get; private set; }

        private PaymentHistory() { }

        public PaymentHistory(Guid userId, Guid productId, string stripePaymentId, decimal amount, IntervalEnum interval)
        {
            if (string.IsNullOrWhiteSpace(stripePaymentId))
                throw new ArgumentException("Stripe Payment Id cannot be empty", nameof(stripePaymentId));

            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero", nameof(amount));

            Id = Guid.NewGuid();
            UserId = userId;
            ProductId = productId;
            StripePaymentId = stripePaymentId;
            Amount = amount;
            Interval = interval;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
