using Dynamiq.API.Extension.Enums;

namespace Dynamiq.API.Mapping.DTOs
{
    public class PaymentHistoryDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string StripePaymentId { get; set; }
        public decimal Amount { get; set; }
        public IntervalEnum Interval { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public UserDto User { get; set; }
        public Guid UserId { get; set; }
        public ProductDto Product { get; set; }
        public Guid ProductId { get; set; }
    }
}
