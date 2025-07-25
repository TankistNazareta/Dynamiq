using Dynamiq.Domain.Enums;

namespace Dynamiq.Domain.Entities
{
    public class Subscription
    {
        public Guid Id { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        public Guid ProductId { get; private set; }
        public Guid UserId { get; private set; }
        public Guid PaymentHistoryId { get; private set; }

        private Subscription() { } // EF Core

        public Subscription(Guid userId, Guid productId, Guid paymentHistoryId, IntervalEnum interval)
        {
            switch (interval)
            {
                case IntervalEnum.Yearly:
                    EndDate = DateTime.UtcNow.AddYears(1);
                    break;
                case IntervalEnum.Mountly:
                    EndDate = DateTime.UtcNow.AddMonths(1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(interval),
                        $"Unsupported interval type: {interval}");
            }

            Id = Guid.NewGuid();
            UserId = userId;
            ProductId = productId;
            PaymentHistoryId = paymentHistoryId;
            StartDate = DateTime.UtcNow;
        }

        public bool IsActive()
        {
            var now = DateTime.UtcNow;
            return StartDate <= now && EndDate >= now;
        }

        public void Extend(TimeSpan duration)
        {
            if (duration.TotalDays <= 0)
                throw new ArgumentException("Duration must be positive");

            EndDate = EndDate.Add(duration);
        }

        public void Cancel()
        {
            EndDate = DateTime.UtcNow;
        }
    }
}
