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
        public ICollection<PaymentHistory> PaymentHistories { get; set; }
    }
}
