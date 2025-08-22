namespace Dynamiq.Domain.ValueObject
{
    public class ProductParagraph
    {
        public string Text { get; private set; }
        public int Order { get; private set; }

        private ProductParagraph() { }

        public ProductParagraph(string text, int order)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Paragraph cannot be empty");

            Text = text;
            Order = order;
        }

    }
}
