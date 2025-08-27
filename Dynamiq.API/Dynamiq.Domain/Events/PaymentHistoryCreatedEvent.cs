using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Common;
using Dynamiq.Domain.Entities;

namespace Dynamiq.Domain.Events
{
    public class PaymentHistoryCreatedEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; private set; }
        public PaymentHistory PaymentHistory{ get; private set; }

        public PaymentHistoryCreatedEvent(PaymentHistory paymentHistory)
        {
            OccurredOn = DateTime.UtcNow;
            PaymentHistory = paymentHistory;
        }
    }
}
