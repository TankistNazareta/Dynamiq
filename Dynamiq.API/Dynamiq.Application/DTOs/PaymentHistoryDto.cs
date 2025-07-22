using Dynamiq.Domain.Enums;

namespace Dynamiq.Application.DTOs
{
    public class PaymentHistoryDto
    {
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        public string StripePaymentId { get; set; }
        public decimal Amount { get; set; }
        public IntervalEnum Interval { get; set; }
    }
}
