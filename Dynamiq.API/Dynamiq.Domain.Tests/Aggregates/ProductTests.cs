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
        private const int ValidPrice = 100;
        private static readonly Guid ValidCategoryId = Guid.NewGuid();
        private const string ValidImgUrl = "https://example.com/image.jpg";

        [Theory]
        [InlineData(null, ValidStripePriceId, ValidName, ValidDescription, ValidPrice)]
        [InlineData("", ValidStripePriceId, ValidName, ValidDescription, ValidPrice)]
        [InlineData("   ", ValidStripePriceId, ValidName, ValidDescription, ValidPrice)]
        public void Update_ShouldThrow_When_StripeProductIdIsInvalid(
            string stripeProductId,
            string stripePriceId,
            string name,
            string description,
            int price)
        {
            var prod = new Product(ValidStripeProductId, ValidStripePriceId, ValidName, ValidDescription, ValidPrice, IntervalEnum.OneTime, ValidCategoryId, ValidImgUrl);

            Action act = () => prod.Update(stripeProductId, stripePriceId, name, description, price, IntervalEnum.OneTime, ValidCategoryId, ValidImgUrl);

            act.Should().Throw<ArgumentException>().WithMessage("*StripeProductId cannot be empty*");
        }

        [Theory]
        [InlineData(ValidStripeProductId, null, ValidName, ValidDescription, ValidPrice)]
        [InlineData(ValidStripeProductId, "", ValidName, ValidDescription, ValidPrice)]
        [InlineData(ValidStripeProductId, "   ", ValidName, ValidDescription, ValidPrice)]
        public void Update_ShouldThrow_When_StripePriceIdIsInvalid(
            string stripeProductId,
            string stripePriceId,
            string name,
            string description,
            int price)
        {
            var prod = new Product(ValidStripeProductId, ValidStripePriceId, ValidName, ValidDescription, ValidPrice, IntervalEnum.OneTime, ValidCategoryId, ValidImgUrl);

            Action act = () => prod.Update(stripeProductId, stripePriceId, name, description, price, IntervalEnum.OneTime, ValidCategoryId, ValidImgUrl);

            act.Should().Throw<ArgumentException>().WithMessage("*StripePriceId cannot be empty*");
        }

        [Theory]
        [InlineData(ValidStripeProductId, ValidStripePriceId, null, ValidDescription, ValidPrice)]
        [InlineData(ValidStripeProductId, ValidStripePriceId, "", ValidDescription, ValidPrice)]
        [InlineData(ValidStripeProductId, ValidStripePriceId, "   ", ValidDescription, ValidPrice)]
        public void Update_ShouldThrow_When_NameIsInvalid(
            string stripeProductId,
            string stripePriceId,
            string name,
            string description,
            int price)
        {
            var prod = new Product(ValidStripeProductId, ValidStripePriceId, ValidName, ValidDescription, ValidPrice, IntervalEnum.OneTime, ValidCategoryId, ValidImgUrl);

            Action act = () => prod.Update(stripeProductId, stripePriceId, name, description, price, IntervalEnum.OneTime, ValidCategoryId, ValidImgUrl);

            act.Should().Throw<ArgumentException>().WithMessage("*Name cannot be empty*");
        }

        [Theory]
        [InlineData(ValidStripeProductId, ValidStripePriceId, ValidName, ValidDescription, 0)]
        [InlineData(ValidStripeProductId, ValidStripePriceId, ValidName, ValidDescription, -1)]
        public void Update_ShouldThrow_When_PriceIsNotPositive(
            string stripeProductId,
            string stripePriceId,
            string name,
            string description,
            int price)
        {
            var prod = new Product(ValidStripeProductId, ValidStripePriceId, ValidName, ValidDescription, ValidPrice, IntervalEnum.OneTime, ValidCategoryId, ValidImgUrl);

            Action act = () => prod.Update(stripeProductId, stripePriceId, name, description, price, IntervalEnum.OneTime, ValidCategoryId, ValidImgUrl);

            act.Should().Throw<ArgumentException>().WithMessage("*Price must be greater than zero*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("invalid-url")]
        public void Update_ShouldThrow_When_ImgUrlIsInvalid(string imgUrl)
        {
            var prod = new Product(ValidStripeProductId, ValidStripePriceId, ValidName, ValidDescription, ValidPrice, IntervalEnum.OneTime, ValidCategoryId, ValidImgUrl);

            Action act = () => prod.Update(ValidStripeProductId, ValidStripePriceId, ValidName, ValidDescription, ValidPrice, IntervalEnum.OneTime, ValidCategoryId, imgUrl);

            act.Should().Throw<ArgumentException>().WithMessage("*ImgUrl must be a valid URL*");
        }

        [Fact]
        public void Update_ShouldSet_ImgUrl_When_Valid()
        {
            var prod = new Product(ValidStripeProductId, ValidStripePriceId, ValidName, ValidDescription, ValidPrice, IntervalEnum.OneTime, ValidCategoryId, ValidImgUrl);

            var newImgUrl = "https://example.com/new-image.png";

            prod.Update(ValidStripeProductId, ValidStripePriceId, ValidName, ValidDescription, ValidPrice, IntervalEnum.OneTime, ValidCategoryId, newImgUrl);

            prod.ImgUrl.Should().Be(newImgUrl);
        }

        [Fact]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            var product = new Product(ValidStripeProductId, ValidStripePriceId, ValidName, ValidDescription, ValidPrice, IntervalEnum.Monthly, ValidCategoryId, ValidImgUrl);

            product.StripeProductId.Should().Be(ValidStripeProductId);
            product.StripePriceId.Should().Be(ValidStripePriceId);
            product.Name.Should().Be(ValidName);
            product.Description.Should().Be(ValidDescription);
            product.Price.Should().Be(ValidPrice);
            product.Interval.Should().Be(IntervalEnum.Monthly);
            product.CategoryId.Should().Be(ValidCategoryId);
            product.ImgUrl.Should().Be(ValidImgUrl);
            product.ProductPaymentHistories.Should().BeEmpty();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void ChangePrice_ShouldThrow_When_NewPriceIsNotPositive(int newPrice)
        {
            var product = new Product(ValidStripeProductId, ValidStripePriceId, ValidName, ValidDescription, ValidPrice, IntervalEnum.OneTime, ValidCategoryId, ValidImgUrl);

            Action act = () => product.ChangePrice(newPrice);

            act.Should().Throw<ArgumentException>().WithMessage("*Price must be greater than zero*");
        }

        [Fact]
        public void ChangePrice_ShouldUpdatePrice_When_NewPriceIsValid()
        {
            var product = new Product(ValidStripeProductId, ValidStripePriceId, ValidName, ValidDescription, ValidPrice, IntervalEnum.OneTime, ValidCategoryId, ValidImgUrl);

            product.ChangePrice(250);

            product.Price.Should().Be(250);
        }
    }
}
