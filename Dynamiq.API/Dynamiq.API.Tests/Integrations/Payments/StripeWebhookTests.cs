using Dynamiq.Application.DTOs;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Enums;
using Dynamiq.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Text;

namespace Dynamiq.API.Tests.Integrations.Payments
{
    public class StripeWebhookTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public StripeWebhookTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        private async Task CreatePaymentHistory(WebhookParserDto parserDto)
        {
            var mockParser = new Mock<IStripeWebhookParser>();
            mockParser.Setup(p => p.ParseCheckoutSessionCompleted(It.IsAny<string>(), It.IsAny<string>()))
                      .Returns(parserDto);

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IStripeWebhookParser));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddSingleton(mockParser.Object);
                });
            }).CreateClient();

            var fakeJson = @"{
              ""type"": ""checkout.session.completed"",
              ""data"": {
                ""object"": {
                  ""id"": ""cs_test_123"",
                  ""client_reference_id"": ""user-id"",
                  ""amount_total"": 200000,
                  ""metadata"": {
                    ""WebhookParserDto"": ""{}""
                  }
                }
              }
            }";

            var fakeSignature = "t=123456789,v1=fakesignature";

            client.DefaultRequestHeaders.Add("Stripe-Signature", fakeSignature);

            var content = new StringContent(fakeJson, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/payment/webhook", content);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadAsStringAsync();
            result.Should().Be("completed successfully");
        }

        [Fact]
        public async Task StripeWebhookHandler_ShouldProcessCartCheckoutSessionCompleted_AndSavePaymentHistory()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var user = new User($"user_{Guid.NewGuid():N}@test.com", "hashedpass", RoleEnum.DefaultUser);
            db.Users.Add(user);

            var category = new Category($"Test Category For {Guid.NewGuid():N}");
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            var product = new Product("product_test_123", "price_test_123", "TestProduct", "test descr", 2000, IntervalEnum.OneTime, category.Id);
            db.Products.Add(product);
            await db.SaveChangesAsync();

            var cart = new Cart(user.Id);
            cart.AddItem(product.Id, 2);
            db.Carts.Add(cart);

            await db.SaveChangesAsync();

            var parserDto = new WebhookParserDto()
            {
                UserId = user.Id,
                Interval = product.Interval,
                StripePaymentId = "test_stripe_id",
                Amount = 4000
            };

            await CreatePaymentHistory(parserDto);

            db.ChangeTracker.Clear();

            var paymentHistory = await db.PaymentHistories
                .Where(ph => ph.UserId == user.Id)
                .ToListAsync();

            paymentHistory.Should().HaveCount(1);
            paymentHistory[0].Amount.Should().Be(4000);

            var productsPaymentHistory = await db.ProductPaymentHistories
                .Where(pph => pph.PaymentHistoryId == paymentHistory[0].Id)
                .ToListAsync();

            productsPaymentHistory.Should().HaveCount(1);
            productsPaymentHistory[0].Quantity.Should().Be(2);
        }


        [Fact]
        public async Task StripeWebhookHandler_ShouldProcessProductCheckoutSessionCompleted_AndSavePaymentHistory()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var user = new User($"user_{Guid.NewGuid():N}@test.com", "hashedpass", RoleEnum.DefaultUser);
            db.Users.Add(user);

            var category = new Category($"Test Category For {Guid.NewGuid():N}");
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            var product = new Product("product_test_123", "price_test_123", "TestProduct", "test descr", 2000, IntervalEnum.OneTime, category.Id);
            db.Products.Add(product);
            await db.SaveChangesAsync();

            await db.SaveChangesAsync();

            var parserDto = new WebhookParserDto()
            {
                UserId = user.Id,
                ProductId = product.Id,
                Interval = product.Interval,
                StripePaymentId = "test_stripe_id",
                Amount = 2000
            };

            await CreatePaymentHistory(parserDto);

            db.ChangeTracker.Clear();

            var paymentHistory = await db.PaymentHistories
                .Where(ph => ph.UserId == user.Id)
                .ToListAsync();

            paymentHistory.Should().HaveCount(1);
            paymentHistory[0].Amount.Should().Be(2000);

            var productsPaymentHistory = await db.ProductPaymentHistories
                .Where(pph => pph.PaymentHistoryId == paymentHistory[0].Id)
                .ToListAsync();

            productsPaymentHistory.Should().HaveCount(1);
        }

        [Fact]
        public async Task StripeWebhookHandler_ShouldProcessSubscriptionCheckoutSessionCompleted_AndSavePaymentHistory()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var user = new User($"user_{Guid.NewGuid():N}@test.com", "hashedpass", RoleEnum.DefaultUser);
            db.Users.Add(user);

            var category = new Category($"{Guid.NewGuid():N} Category");
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            var product = new Domain.Aggregates.Product("product_test_123", "price_test_123", "TestProduct", "test descr", 2000, IntervalEnum.Monthly, category.Id);
            db.Products.Add(product);
            await db.SaveChangesAsync();

            await db.SaveChangesAsync();

            var parserDto = new WebhookParserDto()
            {
                UserId = user.Id,
                ProductId = product.Id,
                Interval = product.Interval,
                StripePaymentId = "test_stripe_id",
                Amount = 2000
            };

            await CreatePaymentHistory(parserDto);

            db.ChangeTracker.Clear();

            var paymentHistory = await db.PaymentHistories
                .Where(ph => ph.UserId == user.Id)
                .ToListAsync();

            paymentHistory.Should().HaveCount(1);
            paymentHistory[0].Amount.Should().Be(2000);

            var productsPaymentHistory = await db.ProductPaymentHistories
                .Where(pph => pph.PaymentHistoryId == paymentHistory[0].Id)
                .ToListAsync();

            productsPaymentHistory.Should().HaveCount(1);

            var userUpdated = await db.Users
                .Include(u => u.Subscriptions)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            userUpdated.Subscriptions.Should().HaveCount(1);

            var subscription = userUpdated.Subscriptions.First();
            subscription.Should().Match<Domain.Entities.Subscription>(s => s.IsActive());
        }
    }
}
