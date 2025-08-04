using Dynamiq.Domain.Aggregates;
using FluentAssertions;

namespace Dynamiq.Domain.Tests.Aggregates
{
    public class CartTests
    {
        private readonly Guid _userId = Guid.NewGuid();

        [Fact]
        public void ShouldAddItemWhenNewItem()
        {
            var cart = new Cart(_userId);
            var productId = Guid.NewGuid();

            cart.AddItem(productId, 2);

            cart.Items.Should().ContainSingle(item =>
                item.ProductId == productId && item.Quantity == 2);
        }

        [Fact]
        public void ShouldIncreaseQuantityWhenAddingExistingItem()
        {
            var cart = new Cart(_userId);
            var productId = Guid.NewGuid();
            cart.AddItem(productId, 2);

            cart.AddItem(productId, 3);

            cart.Items.Should().ContainSingle(item =>
                item.ProductId == productId && item.Quantity == 5);
        }

        [Fact]
        public void ShouldDecreaseQuantityWhenRemovingLessThanExisting()
        {
            var cart = new Cart(_userId);
            var productId = Guid.NewGuid();
            cart.AddItem(productId, 5);

            cart.RemoveItem(productId, 2);

            cart.Items.Should().ContainSingle(item =>
                item.ProductId == productId && item.Quantity == 3);
        }

        [Fact]
        public void ShouldRemoveItemWhenRemovingEqualOrMoreThanExisting()
        {
            var cart = new Cart(_userId);
            var productId = Guid.NewGuid();
            cart.AddItem(productId, 3);

            cart.RemoveItem(productId, 5);

            cart.Items.Should().NotContain(item =>
                item.ProductId == productId);
        }

        [Fact]
        public void ShouldClearAllItemsWhenClearingCart()
        {
            var cart = new Cart(_userId);
            cart.AddItem(Guid.NewGuid(), 1);
            cart.AddItem(Guid.NewGuid(), 2);

            cart.Clear();

            cart.Items.Should().BeEmpty();
        }
    }
}
