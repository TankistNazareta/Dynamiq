using Dynamiq.API.Extension.Enums;

namespace Dynamiq.API.DAL.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string StripeProductId { get; set; }
        public string StripePriceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public IntervalEnum Interval { get; set; }
        public ICollection<PaymentHistory> PaymentHistories { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; }
    }
}
