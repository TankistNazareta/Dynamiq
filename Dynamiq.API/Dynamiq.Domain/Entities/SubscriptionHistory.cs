using Dynamiq.Domain.Enums;

namespace Dynamiq.Domain.Entities
{
    public class SubscriptionHistory
    {
        public Guid Id { get; private set; }
        public DateTime StartDate { get; private set; }
        public bool IsActive { get; private set; }

        public Guid SubscriptionId { get; private set; }
        public Guid PaymentHistoryId { get; private set; }

        private SubscriptionHistory() { } // EF Core

        public SubscriptionHistory(Guid subscriptionId, Guid paymentHistoryId)
        {
            SubscriptionId = subscriptionId;
            PaymentHistoryId = paymentHistoryId;
            StartDate = DateTime.UtcNow;
            IsActive = true;
        }

        public void Cancel()
        {
            IsActive = false;
        }
    }
}
