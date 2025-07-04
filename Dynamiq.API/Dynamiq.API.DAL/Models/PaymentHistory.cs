using Dynamiq.API.Extension.Enums;

namespace Dynamiq.API.DAL.Models
{
    public class PaymentHistory
    {
        public Guid Id { get; set; }
        public string StripePaymentId { get; set; }
        public decimal Amount { get; set; }
        public PaymentTypeEnum PaymentType { get; set; }
        public DateTime CreatedAt { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public Product Product { get; set; }
        public Guid ProductId { get; set; }
    }
}
