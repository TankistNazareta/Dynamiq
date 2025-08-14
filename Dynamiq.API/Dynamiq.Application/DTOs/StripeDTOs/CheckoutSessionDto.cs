using Dynamiq.Domain.Enums;

namespace Dynamiq.Application.DTOs.StripeDTOs
{
    public class CheckoutSessionDto
    {
        public Guid? ProductId { get; set; }
        public Guid? CartId { get; set; }
        public string SuccessUrl { get; set; }
        public string CancelUrl { get; set; }
        public IntervalEnum Interval { get; set; }
        public Guid UserId { get; set; }
    }
}
