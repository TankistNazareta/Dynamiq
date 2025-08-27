using Dynamiq.Domain.Enums;
using Dynamiq.Domain.ValueObject;

namespace Dynamiq.Domain.Aggregates
{
    public class Product
    {
        public Guid Id { get; private set; }
        public string StripeProductId { get; private set; }
        public string StripePriceId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string CardDescription { get; private set; }
        public int Price { get; private set; }
        public IntervalEnum Interval { get; private set; }

        private List<ProductImgUrl> _imgUrls = new();
        public IReadOnlyList<ProductImgUrl> ImgUrls => _imgUrls.AsReadOnly();

        private List<ProductParagraph> _paragraphs = new();
        public IReadOnlyList<ProductParagraph> Paragraphs => _paragraphs
            .OrderBy(p => p.Order)
            .ToList()
            .AsReadOnly();

        public Guid CategoryId { get; private set; }

        private Product() { }

        public Product(
            string stripeProductId,
            string stripePriceId,
            string name,
            string description,
            int price,
            IntervalEnum interval,
            Guid categoryId,
            List<string> imgUrls,
            List<string> paragraphs,
            string cardDescription)
        {
            Update(stripeProductId, stripePriceId, name, description, price, interval, categoryId, imgUrls, paragraphs, cardDescription);
            CardDescription = cardDescription;
        }

        public void Update(
            string stripeProductId,
            string stripePriceId,
            string name,
            string description,
            int price,
            IntervalEnum interval,
            Guid categoryId,
            List<string> imgUrls,
            List<string> paragraphs,
            string cardDescr)
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
            CardDescription = cardDescr;

            _imgUrls.Clear();
            foreach (var url in imgUrls)
                AddImgUrl(url);

            _paragraphs.Clear();
            for (int i = 0; i < paragraphs.Count; i++)
                AddParagraph(paragraphs[i], i);
        }

        private void AddImgUrl(string imgUrl)
        {
            if (string.IsNullOrWhiteSpace(imgUrl) ||
                !Uri.TryCreate(imgUrl, UriKind.Absolute, out var uriResult) ||
                (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException("ImgUrl must be a valid URL");
            }

            _imgUrls.Add(new(imgUrl));
        }

        private void AddParagraph(string text, int order)
        {
            _paragraphs.Add(new ProductParagraph(text, order));
        }

        public void ChangePrice(int newPrice)
        {
            if (newPrice <= 0)
                throw new ArgumentException("Price must be greater than zero");

            Price = newPrice;
        }
    }
}
