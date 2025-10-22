using Dynamiq.Domain.Enums;

namespace Dynamiq.Domain.Aggregates
{
    public class Subscription
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public IntervalEnum Interval { get; private set; }
        public int Price { get; private set; }
        public string StripePriceId { get; private set; }
        public string StripeProductId { get; private set; }

        //For EF
        public Subscription() { }

        public Subscription(string name, IntervalEnum interval, int price, string stripePriceId, string stripeProductId)
        {
            Update(name, interval, price, stripePriceId, stripeProductId);
        }

        public void Update(string name, IntervalEnum interval, int price, string stripePriceId, string stripeProductId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));
            if (price <= 0)
                throw new ArgumentException("Price must be greater than zero", nameof(price));

            Name = name;
            Interval = interval;
            Price = price;
            StripePriceId = stripePriceId;
            StripeProductId = stripeProductId;
        }
    }
}
