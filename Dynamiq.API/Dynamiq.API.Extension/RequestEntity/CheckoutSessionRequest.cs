using Dynamiq.API.Extension.Enums;

namespace Dynamiq.API.Extension.RequestEntity
{
    public class CheckoutSessionRequest
    {
        public Guid ProductId { get; set; }
        public string StripePriceId { get; set; }
        public string SuccessUrl { get; set; }
        public string CancelUrl { get; set; }
        public int Quantity { get; set; } = 1;
        public PaymentTypeEnum PaymentTypeEnum { get; set; }
        public Guid UserId { get; set; }
    }
}
