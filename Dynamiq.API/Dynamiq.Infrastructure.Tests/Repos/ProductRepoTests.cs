using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Common;
using Dynamiq.Domain.Enums;
using Dynamiq.Infrastructure.Persistence.Context;
using Dynamiq.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;


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
                new Product(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Product A", "Desc A", 100, IntervalEnum.OneTime, categoryId),
                new Product(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Product B", "Desc B", 200, IntervalEnum.OneTime, Guid.NewGuid()),
                new Product(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Product C", "Desc C", 300, IntervalEnum.OneTime, categoryId),
            };

            await _dbContext.Products.AddRangeAsync(products);
            await _dbContext.SaveChangesAsync();

            var filter = new ProductFilter { CategoryId = categoryId };

            var result = await _repo.GetFilteredAsync(filter, CancellationToken.None);

            Assert.All(result, p => Assert.Equal(categoryId, p.CategoryId));
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetFilteredAsync_ShouldFilterByMinPrice()
        {
            var products = new List<Product>
            {
                new Product(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Product A", "Desc A", 50, IntervalEnum.OneTime, Guid.NewGuid()),
                new Product(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Product B", "Desc B", 150, IntervalEnum.OneTime, Guid.NewGuid()),
                new Product(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Product C", "Desc C", 250, IntervalEnum.OneTime, Guid.NewGuid()),
            };

            await _dbContext.Products.AddRangeAsync(products);
            await _dbContext.SaveChangesAsync();

            var filter = new ProductFilter { MinPrice = 100 };

            var result = await _repo.GetFilteredAsync(filter, CancellationToken.None);

            Assert.All(result, p => Assert.True(p.Price >= 100));
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetFilteredAsync_ShouldFilterByMaxPrice()
        {
            var products = new List<Product>
            {
                new Product(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Product A", "Desc A", 50, IntervalEnum.OneTime, Guid.NewGuid()),
                new Product(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Product B", "Desc B", 150, IntervalEnum.OneTime, Guid.NewGuid()),
                new Product(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Product C", "Desc C", 250, IntervalEnum.OneTime, Guid.NewGuid()),
            };

            await _dbContext.Products.AddRangeAsync(products);
            await _dbContext.SaveChangesAsync();

            var filter = new ProductFilter { MaxPrice = 200 };

            var result = await _repo.GetFilteredAsync(filter, CancellationToken.None);

            Assert.All(result, p => Assert.True(p.Price <= 200));
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetFilteredAsync_ShouldFilterBySearchTerm()
        {
            var products = new List<Product>
            {
                new Product(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Apple iPhone", "Smartphone", 999, IntervalEnum.OneTime, Guid.NewGuid()),
                new Product(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Samsung Galaxy", "Android phone", 799, IntervalEnum.OneTime, Guid.NewGuid()),
                new Product(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Google Pixel", "Android phone", 699, IntervalEnum.OneTime, Guid.NewGuid()),
            };

            await _dbContext.Products.AddRangeAsync(products);
            await _dbContext.SaveChangesAsync();

            var filter = new ProductFilter { SearchTerm = "android" };

            var result = await _repo.GetFilteredAsync(filter, CancellationToken.None);

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
                new Product(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Product A", "Desc A", 100, IntervalEnum.OneTime, Guid.NewGuid()),
                new Product(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Product B", "Desc B", 200, IntervalEnum.OneTime, Guid.NewGuid())
            };

            await _dbContext.Products.AddRangeAsync(products);
            await _dbContext.SaveChangesAsync();

            var filter = new ProductFilter();

            var result = await _repo.GetFilteredAsync(filter, CancellationToken.None);

            Assert.Equal(products.Count, result.Count);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
