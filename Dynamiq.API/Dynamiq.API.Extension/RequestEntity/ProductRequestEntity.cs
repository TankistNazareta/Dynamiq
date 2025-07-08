using Dynamiq.API.Extension.Enums;

namespace Dynamiq.API.Extension.RequestEntity
{
    public class ProductRequestEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public IntervalEnum PaymentType { get; set; }
    }
}
