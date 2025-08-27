using Dynamiq.Domain.Common;
using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Enums;
using Dynamiq.Domain.Events;
using Dynamiq.Domain.ValueObject;

namespace Dynamiq.Domain.Aggregates
{
    public class PaymentHistory : BaseEntity
    {
        public Guid Id { get; private set; }
        public string StripePaymentId { get; private set; }
        public decimal Amount { get; private set; }
        public IntervalEnum Interval { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Guid UserId { get; private set; }

        private readonly List<ProductPaymentHistory> _products = new();
        public IReadOnlyCollection<ProductPaymentHistory> Products => _products.AsReadOnly();

        private PaymentHistory() { } // EF Core

        public PaymentHistory(Guid userId, string stripePaymentId, decimal amount, IntervalEnum interval)
        {
            if (string.IsNullOrWhiteSpace(stripePaymentId))
                throw new ArgumentException("Stripe Payment Id cannot be empty", nameof(stripePaymentId));

            if (amount <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(amount));

            UserId = userId;
            StripePaymentId = stripePaymentId;
            Amount = amount;
            Interval = interval;
            CreatedAt = DateTime.UtcNow;

            AddDomainEvent(new PaymentHistoryCreatedEvent(this));
        }

        public void AddProduct(Guid productId, int quantity = 1)
            => _products.Add(new(productId, Id, quantity));
    }
}
