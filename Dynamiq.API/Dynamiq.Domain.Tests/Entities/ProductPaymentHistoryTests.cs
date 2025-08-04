using Dynamiq.Domain.Entities;
using FluentAssertions;

namespace Dynamiq.Domain.Tests.Entities
{
    public class ProductPaymentHistoryTests
    {
        private readonly Guid _productId = Guid.NewGuid();
        private readonly Guid _paymentHistoryId = Guid.NewGuid();

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_ShouldThrow_When_QuantityNotPositive(int quantity)
        {
            Action act = () => new ProductPaymentHistory(_productId, _paymentHistoryId, quantity);
            act.Should().Throw<ArgumentException>()
                .WithParameterName("quantity");
        }

        [Fact]
        public void Constructor_ShouldSetProperties_When_ValidArguments()
        {
            var pph = new ProductPaymentHistory(_productId, _paymentHistoryId, 3);
            pph.ProductId.Should().Be(_productId);
            pph.PaymentHistoryId.Should().Be(_paymentHistoryId);
            pph.Quantity.Should().Be(3);
            pph.Id.Should().BeEmpty();
        }
    }
}
