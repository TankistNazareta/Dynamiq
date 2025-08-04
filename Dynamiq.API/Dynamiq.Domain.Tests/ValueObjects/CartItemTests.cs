using Dynamiq.Domain.ValueObject;
using FluentAssertions;

namespace Dynamiq.Domain.Tests.ValueObjects
{
    public class CartItemTests
    {
        [Fact]
        public void Constructor_ShouldSetProperties()
        {
            var productId = Guid.NewGuid();
            var quantity = 4;

            var item = new CartItem(productId, quantity);

            item.ProductId.Should().Be(productId);
            item.Quantity.Should().Be(quantity);
        }

        [Fact]
        public void IncreaseQuantity_ShouldAddAmount()
        {
            var item = new CartItem(Guid.NewGuid(), 2);

            item.IncreaseQuantity(3);

            item.Quantity.Should().Be(5);
        }

        [Fact]
        public void SetQuantity_ShouldOverrideQuantity()
        {
            var item = new CartItem(Guid.NewGuid(), 5);

            item.SetQuantity(1);

            item.Quantity.Should().Be(1);
        }
    }
}
