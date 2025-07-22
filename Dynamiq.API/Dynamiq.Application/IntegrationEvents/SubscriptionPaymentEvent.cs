using Dynamiq.Domain.Common;
using Dynamiq.Domain.Enums;
using Dynamiq.Domain.Enums;

namespace Dynamiq.Domain.Events
{
    public class SubscriptionPaymentEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; }
        public IntervalEnum Interval { get; }

        public Guid ProductId { get; }
        public Guid UserId { get; }
        public Guid PaymentHistoryId { get; }

        public SubscriptionPaymentEvent(Guid paymentHistoryId, Guid userId, Guid productId, IntervalEnum interval)
        {
            OccurredOn = DateTime.UtcNow;

            PaymentHistoryId = paymentHistoryId;
            UserId = userId;
            ProductId = productId;
            Interval = interval;
        }
    }
}
