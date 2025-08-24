using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Enums;
using FluentAssertions;

namespace Dynamiq.Domain.Tests.Aggregates
{
    public class ProductTests
    {
        private const string ValidStripeProductId = "prod_123";
        private const string ValidStripePriceId = "price_456";
        private const string ValidName = "Test Product";
        private const string ValidDescription = "Description";
        private const string ValidCardDescription = "Card short description";
        private const int ValidPrice = 100;
        private static readonly Guid ValidCategoryId = Guid.NewGuid();
        private readonly List<string> ValidImgUrls = new() { "https://example.com/image.jpg" };
        private readonly List<string> ValidParagraphs = new()
        {
            "First paragraph",
            "Second paragraph"
        };

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Update_ShouldThrow_When_StripeProductIdIsInvalid(string invalidId)
        {
            var prod = CreateValidProduct();

            Action act = () => prod.Update(
                invalidId,
                ValidStripePriceId,
                ValidName,
                ValidDescription,
                ValidPrice,
                IntervalEnum.OneTime,
                ValidCategoryId,
                ValidImgUrls,
                ValidParagraphs,
                ValidCardDescription);

            act.Should().Throw<ArgumentException>().WithMessage("*StripeProductId cannot be empty*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Update_ShouldThrow_When_StripePriceIdIsInvalid(string invalidId)
        {
            var prod = CreateValidProduct();

            Action act = () => prod.Update(
                ValidStripeProductId,
                invalidId,
                ValidName,
                ValidDescription,
                ValidPrice,
                IntervalEnum.OneTime,
                ValidCategoryId,
                ValidImgUrls,
                ValidParagraphs,
                ValidCardDescription);

            act.Should().Throw<ArgumentException>().WithMessage("*StripePriceId cannot be empty*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Update_ShouldThrow_When_NameIsInvalid(string invalidName)
        {
            var prod = CreateValidProduct();

            Action act = () => prod.Update(
                ValidStripeProductId,
                ValidStripePriceId,
                invalidName,
                ValidDescription,
                ValidPrice,
                IntervalEnum.OneTime,
                ValidCategoryId,
                ValidImgUrls,
                ValidParagraphs,
                ValidCardDescription);

            act.Should().Throw<ArgumentException>().WithMessage("*Name cannot be empty*");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Update_ShouldThrow_When_PriceIsNotPositive(int price)
        {
            var prod = CreateValidProduct();

            Action act = () => prod.Update(
                ValidStripeProductId,
                ValidStripePriceId,
                ValidName,
                ValidDescription,
                price,
                IntervalEnum.OneTime,
                ValidCategoryId,
                ValidImgUrls,
                ValidParagraphs,
                ValidCardDescription);

            act.Should().Throw<ArgumentException>().WithMessage("*Price must be greater than zero*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("invalid-url")]
        public void Update_ShouldThrow_When_ImgUrlIsInvalid(string imgUrl)
        {
            var prod = CreateValidProduct();

            Action act = () => prod.Update(
                ValidStripeProductId,
                ValidStripePriceId,
                ValidName,
                ValidDescription,
                ValidPrice,
                IntervalEnum.OneTime,
                ValidCategoryId,
                new List<string> { imgUrl },
                ValidParagraphs,
                ValidCardDescription);

            act.Should().Throw<ArgumentException>().WithMessage("*ImgUrl must be a valid URL*");
        }

        [Fact]
        public void Update_ShouldSet_ImgUrls_When_Valid()
        {
            var prod = CreateValidProduct();

            var newImgUrls = new List<string>
            {
                "https://example.com/new-image.png",
                "https://example.com/another.png"
            };

            prod.Update(
                ValidStripeProductId,
                ValidStripePriceId,
                ValidName,
                ValidDescription,
                ValidPrice,
                IntervalEnum.OneTime,
                ValidCategoryId,
                newImgUrls,
                ValidParagraphs,
                ValidCardDescription);

            prod.ImgUrls.Select(i => i.ImgUrl).Should().BeEquivalentTo(newImgUrls);
        }

        [Fact]
        public void Update_ShouldSet_Paragraphs_When_Valid()
        {
            var prod = CreateValidProduct();

            var newParagraphs = new List<string>
            {
                "Paragraph 1",
                "Paragraph 2",
                "Paragraph 3"
            };

            prod.Update(
                ValidStripeProductId,
                ValidStripePriceId,
                ValidName,
                ValidDescription,
                ValidPrice,
                IntervalEnum.OneTime,
                ValidCategoryId,
                ValidImgUrls,
                newParagraphs,
                ValidCardDescription);

            prod.Paragraphs.Select(p => p.Text).Should().BeEquivalentTo(newParagraphs);
        }

        [Fact]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            var product = CreateValidProduct();

            product.StripeProductId.Should().Be(ValidStripeProductId);
            product.StripePriceId.Should().Be(ValidStripePriceId);
            product.Name.Should().Be(ValidName);
            product.Description.Should().Be(ValidDescription);
            product.CardDescription.Should().Be(ValidCardDescription);
            product.Price.Should().Be(ValidPrice);
            product.Interval.Should().Be(IntervalEnum.OneTime);
            product.CategoryId.Should().Be(ValidCategoryId);
            product.ImgUrls.Select(i => i.ImgUrl).Should().BeEquivalentTo(ValidImgUrls);
            product.Paragraphs.Select(p => p.Text).Should().BeEquivalentTo(ValidParagraphs);
            product.ProductPaymentHistories.Should().BeEmpty();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void ChangePrice_ShouldThrow_When_NewPriceIsNotPositive(int newPrice)
        {
            var product = CreateValidProduct();

            Action act = () => product.ChangePrice(newPrice);

            act.Should().Throw<ArgumentException>().WithMessage("*Price must be greater than zero*");
        }

        [Fact]
        public void ChangePrice_ShouldUpdatePrice_When_NewPriceIsValid()
        {
            var product = CreateValidProduct();

            product.ChangePrice(250);

            product.Price.Should().Be(250);
        }

        private Product CreateValidProduct() =>
            new Product(
                ValidStripeProductId,
                ValidStripePriceId,
                ValidName,
                ValidDescription,
                ValidPrice,
                IntervalEnum.OneTime,
                ValidCategoryId,
                ValidImgUrls,
                ValidParagraphs,
                ValidCardDescription);
    }
}
