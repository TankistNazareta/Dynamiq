using Dynamiq.API.Extension.Enums;

namespace Dynamiq.API.Mapping.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string StripeProductId { get; set; }
        public string StripePriceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public IntervalEnum Interval { get; set; }
        public List<PaymentHistoryDto> PaymentHistories { get; set; } = new ();
    }
}
