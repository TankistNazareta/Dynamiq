using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Common;
using Dynamiq.Domain.Enums;
using Dynamiq.Infrastructure.Persistence.Context;
using Dynamiq.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Dynamiq.Infrastructure.Tests.Repos
{
    public class ProductRepoTests : IDisposable
    {
        private readonly AppDbContext _dbContext;
        private readonly ProductRepo _repo;

        public ProductRepoTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new AppDbContext(options, dispatcher: null);
            _repo = new ProductRepo(_dbContext);
        }

        [Fact]
        public async Task GetFilteredAsync_ShouldFilterByCategoryId()
        {
            var categoryId = Guid.NewGuid();

            var products = new List<Product>
            {
                CreateProduct("Product A", 100, categoryId),
                CreateProduct("Product B", 200, Guid.NewGuid()),
                CreateProduct("Product C", 300, categoryId),
            };

            await _dbContext.Products.AddRangeAsync(products);
            await _dbContext.SaveChangesAsync();

            var filter = new ProductFilter { CategoryId = categoryId };

            var result = await _repo.GetFilteredAsync(filter, 100, 0, CancellationToken.None);

            Assert.All(result, p => Assert.Equal(categoryId, p.CategoryId));
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetFilteredAsync_ShouldFilterByMinPrice()
        {
            var products = new List<Product>
            {
                CreateProduct("Product A", 50),
                CreateProduct("Product B", 150),
                CreateProduct("Product C", 250),
            };

            await _dbContext.Products.AddRangeAsync(products);
            await _dbContext.SaveChangesAsync();

            var filter = new ProductFilter { MinPrice = 100 };

            var result = await _repo.GetFilteredAsync(filter, 100, 0, CancellationToken.None);

            Assert.All(result, p => Assert.True(p.Price >= 100));
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetFilteredAsync_ShouldFilterByMaxPrice()
        {
            var products = new List<Product>
            {
                CreateProduct("Product A", 50),
                CreateProduct("Product B", 150),
                CreateProduct("Product C", 250),
            };

            await _dbContext.Products.AddRangeAsync(products);
            await _dbContext.SaveChangesAsync();

            var filter = new ProductFilter { MaxPrice = 200 };

            var result = await _repo.GetFilteredAsync(filter, 100, 0, CancellationToken.None);

            Assert.All(result, p => Assert.True(p.Price <= 200));
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetFilteredAsync_ShouldFilterBySearchTerm()
        {
            var products = new List<Product>
            {
                CreateProduct("Apple iPhone", 999, null, "Smartphone"),
                CreateProduct("Samsung Galaxy", 799, null,"Android phone"),
                CreateProduct("Google Pixel", 699, null, "Android phone"),
            };

            await _dbContext.Products.AddRangeAsync(products);
            await _dbContext.SaveChangesAsync();

            var filter = new ProductFilter { SearchTerm = "android" };

            var result = await _repo.GetFilteredAsync(filter, 100, 0, CancellationToken.None);

            Assert.All(result, p =>
                Assert.True(
                    p.Name.Contains("Samsung", StringComparison.OrdinalIgnoreCase) ||
                    p.Description.Contains("Android", StringComparison.OrdinalIgnoreCase)
                )
            );
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetFilteredAsync_ShouldReturnAll_WhenFilterIsEmpty()
        {
            var products = new List<Product>
            {
                CreateProduct("Product A", 100),
                CreateProduct("Product B", 200)
            };

            await _dbContext.Products.AddRangeAsync(products);
            await _dbContext.SaveChangesAsync();

            var filter = new ProductFilter();

            var result = await _repo.GetFilteredAsync(filter, 100, 0, CancellationToken.None);

            Assert.Equal(products.Count, result.Count);
        }

        private Product CreateProduct(string name, int price, Guid? categoryId = null, string description = "Description")
        {
            return new Product(
                stripeProductId: Guid.NewGuid().ToString(),
                stripePriceId: Guid.NewGuid().ToString(),
                name: name,
                description: description,
                price: price,
                interval: IntervalEnum.OneTime,
                categoryId: categoryId ?? Guid.NewGuid(),
                imgUrls: new List<string> { "https://example.com/image.jpg" },
                paragraphs: new List<string> { "Paragraph 1" },
                cardDescription: "Card description"
            );
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
