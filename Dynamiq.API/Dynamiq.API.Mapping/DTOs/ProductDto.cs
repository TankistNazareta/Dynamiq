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
        public PaymentTypeEnum PaymentType { get; set; }
        public ICollection<PaymentHistoryDto> PaymentHistories { get; set; } = null;
    }
}
