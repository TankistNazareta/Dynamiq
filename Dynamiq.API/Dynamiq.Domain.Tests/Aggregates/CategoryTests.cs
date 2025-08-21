using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Enums;
using FluentAssertions;

namespace Dynamiq.Domain.Tests.Aggregates
{
    public class CategoryTests
    {
        [Fact]
        public void Should_SetNameAndSlug_When_ConstructedWithoutParent()
        {
            var name = "Test Category";
            var category = new Category(name);

            category.Name.Should().Be(name);
            category.Slug.Should().Be("test-category");
            category.ParentCategoryId.Should().BeNull();
            category.SubCategories.Should().BeEmpty();
            category.Products.Should().BeEmpty();
        }

        [Fact]
        public void Should_SetParentCategoryId_When_ConstructedWithParent()
        {
            var name = "Child";
            var parentId = Guid.NewGuid();
            var category = new Category(name, parentId);

            category.Name.Should().Be(name);
            category.Slug.Should().Be("child");
            category.ParentCategoryId.Should().Be(parentId);
        }

        [Fact]
        public void Should_AddSubCategory()
        {
            var parent = new Category("Parent");
            var child = new Category("Child");

            parent.AddSubCategory(child);

            parent.SubCategories.Should().ContainSingle()
                .Which.Should().Be(child);
        }

        [Fact]
        public void Should_AddProduct()
        {
            var category = new Category("Electronics");
            var product = new Product("TestStripeProductId", "TestStripePriceId", "name", "descr",  20, IntervalEnum.OneTime, category.Id, new() { "https://test.com/img" });

            category.AddProduct(product);

            category.Products.Should().ContainSingle()
                .Which.Should().Be(product);
        }
    }
}
