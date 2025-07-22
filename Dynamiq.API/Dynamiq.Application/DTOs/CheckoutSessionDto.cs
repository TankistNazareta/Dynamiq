using Dynamiq.Domain.Enums;

namespace Dynamiq.Application.DTOs
{
    public class CheckoutSessionDto
    {
        public Guid ProductId { get; set; }
        public string StripePriceId { get; set; }
        public string SuccessUrl { get; set; }
        public string CancelUrl { get; set; }
        public int Quantity { get; set; } = 1;
        public int Amount { get; set; }
        public IntervalEnum Interval { get; set; }
        public Guid UserId { get; set; }
    }
}
