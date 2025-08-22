using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Enums;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

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

            var product = new Product(
                stripeProductId: "TestStripeProductId",
                stripePriceId: "TestStripePriceId",
                name: "name",
                description: "descr",
                price: 20,
                interval: IntervalEnum.OneTime,
                categoryId: category.Id,
                imgUrls: new List<string> { "https://test.com/img" },
                paragraphs: new List<string> { "Paragraph 1" },
                cardDescription: "Card descr"
            );

            category.AddProduct(product);

            category.Products.Should().ContainSingle()
                .Which.Should().Be(product);
        }
    }
}
