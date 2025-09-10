using Dynamiq.Domain.Aggregates;
using FluentAssertions;

namespace Dynamiq.Domain.Tests.Aggregates
{
    public class CartTests
    {
        [Fact]
        public void AddItem_Should_Add_New_Item_When_Not_Exists()
        {
            var cart = new Cart(Guid.NewGuid());
            var productId = Guid.NewGuid();

            cart.AddItem(productId, 2);

            Assert.Single(cart.Items);
            Assert.Equal(2, cart.Items.First().Quantity);
            Assert.Equal(productId, cart.Items.First().ProductId);
        }

        [Fact]
        public void AddItem_Should_Increase_Quantity_When_Item_Exists()
        {
            var cart = new Cart(Guid.NewGuid());
            var productId = Guid.NewGuid();

            cart.AddItem(productId, 2);
            cart.AddItem(productId, 3);

            Assert.Single(cart.Items);
            Assert.Equal(5, cart.Items.First().Quantity);
        }

        [Fact]
        public void AddItem_Should_Throw_When_Quantity_Less_Or_Equal_To_Zero()
        {
            var cart = new Cart(Guid.NewGuid());
            var productId = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => cart.AddItem(productId, 0));
            Assert.Throws<ArgumentException>(() => cart.AddItem(productId, -1));
        }

        [Fact]
        public void SetItemQuantity_Should_Add_Item_When_Not_Exists()
        {
            var cart = new Cart(Guid.NewGuid());
            var productId = Guid.NewGuid();

            cart.SetItemQuantity(productId, 4);

            Assert.Single(cart.Items);
            Assert.Equal(4, cart.Items.First().Quantity);
        }

        [Fact]
        public void SetItemQuantity_Should_Update_Quantity_When_Item_Exists()
        {
            var cart = new Cart(Guid.NewGuid());
            var productId = Guid.NewGuid();

            cart.AddItem(productId, 2);
            cart.SetItemQuantity(productId, 7);

            Assert.Single(cart.Items);
            Assert.Equal(7, cart.Items.First().Quantity);
        }

        [Fact]
        public void SetItemQuantity_Should_Remove_Item_When_Set_To_Zero()
        {
            var cart = new Cart(Guid.NewGuid());
            var productId = Guid.NewGuid();

            cart.AddItem(productId, 2);
            cart.SetItemQuantity(productId, 0);

            Assert.Empty(cart.Items);
        }

        [Fact]
        public void SetItemQuantity_Should_Remove_Item_When_Existing_Quantity_Is_Zero()
        {
            var cart = new Cart(Guid.NewGuid());
            var productId = Guid.NewGuid();

            cart.SetItemQuantity(productId, 1);
            cart.Items.Count.Should().Be(1);

            cart.AddItem(productId, 5);
            cart.SetItemQuantity(productId, 0);

            Assert.Empty(cart.Items);
        }

        [Fact]
        public void Clear_Should_Remove_All_Items()
        {
            var cart = new Cart(Guid.NewGuid());
            cart.AddItem(Guid.NewGuid(), 2);
            cart.AddItem(Guid.NewGuid(), 3);

            cart.Clear();

            Assert.Empty(cart.Items);
        }
    }
}
