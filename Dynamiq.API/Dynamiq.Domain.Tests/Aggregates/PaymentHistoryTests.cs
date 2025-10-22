using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Enums;
using Dynamiq.Domain.ValueObject;
using FluentAssertions;

namespace Dynamiq.Domain.Tests.Aggregates
{
    public class PaymentHistoryTests
    {
        private readonly Guid _userId = Guid.NewGuid();
        private const string ValidStripeId = "pi_123456789";
        private const decimal ValidAmount = 100m;

        [Fact]
        public void Constructor_ShouldThrow_When_StripePaymentIdIsNullOrWhitespace()
        {
            Action actNull = () => new PaymentHistory(_userId, null!, ValidAmount, IntervalEnum.Monthly);
            Action actEmpty = () => new PaymentHistory(_userId, "   ", ValidAmount, IntervalEnum.OneTime);

            actNull.Should().Throw<ArgumentException>()
                .WithParameterName("stripePaymentId");
            actEmpty.Should().Throw<ArgumentException>()
                .WithParameterName("stripePaymentId");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-50)]
        public void Constructor_ShouldThrow_When_AmountIsNotPositive(decimal amount)
        {
            Action act = () => new PaymentHistory(_userId, ValidStripeId, amount, IntervalEnum.OneTime);

            act.Should().Throw<ArgumentException>()
                .WithParameterName("amount");
        }

        [Fact]
        public void Constructor_ShouldSetProperties_When_ValidArguments()
        {
            var before = DateTime.UtcNow;
            var ph = new PaymentHistory(_userId, ValidStripeId, ValidAmount, IntervalEnum.Monthly);
            var after = DateTime.UtcNow;

            ph.UserId.Should().Be(_userId);
            ph.StripeTransactionId.Should().Be(ValidStripeId);
            ph.Amount.Should().Be(ValidAmount);
            ph.Interval.Should().Be(IntervalEnum.Monthly);
            ph.Id.Should().BeEmpty();
            ph.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
            ph.Products.Should().BeEmpty();
        }

        [Fact]
        public void AddProduct_ShouldAddProductToCollection()
        {
            var ph = new PaymentHistory(_userId, ValidStripeId, ValidAmount, IntervalEnum.OneTime);
            var productId = Guid.NewGuid();

            ph.AddProduct(productId);

            ph.Products.Should().ContainSingle()
                .Which.ProductId.Should().Be(productId);
        }
    }
}
