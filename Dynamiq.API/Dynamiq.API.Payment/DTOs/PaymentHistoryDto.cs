using Dynamiq.API.Extension.DTOs;
using Dynamiq.API.Extension.Enums;

namespace Dynamiq.API.Payment.DTOs
{
    public class PaymentHistoryDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string StripePaymentId { get; set; }
        public decimal Amount { get; set; }
        public PaymentTypeEnum PaymentType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public UserDto User { get; set; }
        public Guid UserId { get; set; }
    }
}
