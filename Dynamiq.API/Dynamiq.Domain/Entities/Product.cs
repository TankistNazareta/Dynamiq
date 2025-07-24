using Dynamiq.Domain.Enums;

namespace Dynamiq.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; private set; }
        public string StripeProductId { get; private set; }
        public string StripePriceId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int Price { get; private set; }
        public IntervalEnum Interval { get; private set; }

        private readonly List<PaymentHistory> _paymentHistories = new();
        public IReadOnlyCollection<PaymentHistory> PaymentHistories => _paymentHistories.AsReadOnly();

        private readonly List<Subscription> _subscriptions = new();
        public IReadOnlyCollection<Subscription> Subscriptions => _subscriptions.AsReadOnly();

        public Guid CategoryId { get; private set; }

        private Product() { } // EF Core

        public Product(
            string stripeProductId, string stripePriceId,
            string name, string description,
            int price, IntervalEnum interval,
            Guid categoryId)
        {
            Id = Guid.NewGuid();
            Update(stripeProductId, stripePriceId, name, description, price, interval, categoryId);
        }

        public void Update(
            string stripeProductId,
            string stripePriceId, string name,
            string description, int price,
            IntervalEnum interval, Guid categoryId)
        {
            if (string.IsNullOrWhiteSpace(stripeProductId))
                throw new ArgumentException("StripeProductId cannot be empty");

            if (string.IsNullOrWhiteSpace(stripePriceId))
                throw new ArgumentException("StripePriceId cannot be empty");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty");

            if (price <= 0)
                throw new ArgumentException("Price must be greater than zero");

            StripeProductId = stripeProductId;
            StripePriceId = stripePriceId;
            Name = name;
            Description = description;
            Price = price;
            Interval = interval;
            CategoryId = categoryId;
        }

        public void ChangePrice(int newPrice)
        {
            if (newPrice <= 0)
                throw new ArgumentException("Price must be greater than zero");

            Price = newPrice;
        }

        public void AddPaymentHistory(PaymentHistory history)
        {
            _paymentHistories.Add(history);
        }

        public void AddSubscription(Subscription subscription)
        {
            _subscriptions.Add(subscription);
        }
    }
}
