using Dynamiq.Domain.Enums;

namespace Dynamiq.Application.DTOs.PaymentDTOs
{
    public class WebhookParserDto
    {
        public Guid UserId { get; set; }
        public string StripePaymentId { get; set; }
        public decimal Amount { get; set; }
        public IntervalEnum Interval { get; set; }
        public Guid? ProductId { get; set; }
    }
}
