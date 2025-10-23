using Dynamiq.Application.DTOs.PaymentDTOs;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Entities;
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

        private async Task CreatePaymentHistory(WebhookParserDto parserDto, CouponsResultDto? coupons = null)
        {
            var mockParser = new Mock<IStripeWebhookParser>();
            mockParser.Setup(p => p.ParseCheckoutSessionCompleted(It.IsAny<string>(), It.IsAny<string>()))
                      .Returns(parserDto);
            mockParser.Setup(p => p.TryGetCoupons(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(coupons);
            mockParser.Setup(parserDto => parserDto.GetEventType(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("checkout.session.completed");

            var mockCouponStripeService = new Mock<IStripeCouponService>();
            mockCouponStripeService.Setup(p => p.DeactivateCoupon(It.IsAny<string>()));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptorParser = services.SingleOrDefault(d => d.ServiceType == typeof(IStripeWebhookParser));
                    var descriptorCouponStripe = services.SingleOrDefault(d => d.ServiceType == typeof(IStripeCouponService));

                    if (descriptorParser != null)
                        services.Remove(descriptorParser);
                    if (descriptorCouponStripe != null)
                        services.Remove(descriptorCouponStripe);

                    services.AddSingleton(mockParser.Object);
                    services.AddSingleton(mockCouponStripeService.Object);
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
        }

        [Fact]
        public async Task StripeWebhookHandler_ShouldProcessCartCheckoutSessionCompleted_AndSavePaymentHistory()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var user = new User($"user_{Guid.NewGuid():N}@test.com", "hashedpass", RoleEnum.DefaultUser);
            db.Users.Add(user);
            TestAuthHandler.TestUserId = user.Id;

            var category = new Category($"Test Category For {Guid.NewGuid():N}");
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            var product = new Product(
                stripeProductId: "product_test_123",
                stripePriceId: "price_test_123",
                name: "TestProduct",
                description: "test descr",
                price: 2000,
                categoryId: category.Id,
                imgUrls: new List<string> { "https://example.com/image.jpg" },
                paragraphs: new List<string> { "Paragraph 1" },
                cardDescription: "Card description"
            );

            db.Products.Add(product);
            await db.SaveChangesAsync();

            var cart = new Cart(user.Id);
            cart.AddItem(product.Id, 2);
            db.Carts.Add(cart);

            await db.SaveChangesAsync();

            var parserDto = new WebhookParserDto()
            {
                UserId = user.Id,
                StripeTransactionId = "test_stripe_id",
                Amount = 4000
            };

            await CreatePaymentHistory(parserDto);

            db.ChangeTracker.Clear();

            var paymentHistory = await db.PaymentHistories
                .Include(ph => ph.Products)
                .Where(ph => ph.UserId == user.Id)
                .ToListAsync();

            paymentHistory.Should().HaveCount(1);
            paymentHistory[0].Amount.Should().Be(4000);

            var productsPaymentHistory = paymentHistory[0].Products;

            productsPaymentHistory.Should().HaveCount(1);

            TestAuthHandler.TestUserId = null;
        }


        [Fact]
        public async Task StripeWebhookHandler_ShouldProcessProductCheckoutSessionCompleted_AndSavePaymentHistory()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var user = new User($"user_{Guid.NewGuid():N}@test.com", "hashedpass", RoleEnum.DefaultUser);
            db.Users.Add(user);
            await db.SaveChangesAsync();

            user = await db.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

            TestAuthHandler.TestUserId = user.Id;

            var category = new Category($"Test Category For {Guid.NewGuid():N}");
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            var product = new Product(
                stripeProductId: "product_test_123",
                stripePriceId: "price_test_123",
                name: "TestProduct",
                description: "test descr",
                price: 2222,
                categoryId: category.Id,
                imgUrls: new List<string> { "https://example.com/image.jpg" },
                paragraphs: new List<string> { "Paragraph 1" },
                cardDescription: "Card description"
            );

            db.Products.Add(product);

            await db.SaveChangesAsync();

            var parserDto = new WebhookParserDto()
            {
                UserId = user.Id,
                ProductId = product.Id,
                StripeTransactionId = "test_stripe_id",
                Amount = 2222
            };

            await CreatePaymentHistory(parserDto);

            db.ChangeTracker.Clear();

            var paymentHistory = await db.PaymentHistories
                .Include(ph => ph.Products)
                .Where(ph => ph.UserId == user.Id)
                .ToListAsync();

            paymentHistory.Should().HaveCount(1);
            paymentHistory[0].Amount.Should().Be(2222);

            var productsPaymentHistory = paymentHistory[0].Products;

            productsPaymentHistory.Should().HaveCount(1);

            TestAuthHandler.TestUserId = null;
        }

        [Fact]
        public async Task StripeWebhookHandler_ShouldProcessSubscriptionCheckoutSessionCompleted_AndSavePaymentHistory()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var user = new User($"user_{Guid.NewGuid():N}@test.com", "hashedpass", RoleEnum.DefaultUser);
            db.Users.Add(user);
            await db.SaveChangesAsync();

            user = await db.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

            TestAuthHandler.TestUserId = user.Id;

            var product = new Subscription(
                name: "nadsubscription",
                interval: IntervalEnum.Monthly,
                price: 122,
                stripePriceId: "stripePriceId",
                stripeProductId: "stripeProductId"
            );

            db.Subscriptions.Add(product);

            await db.SaveChangesAsync();

            var parserDto = new WebhookParserDto()
            {
                UserId = user.Id,
                ProductId = product.Id,
                Interval = product.Interval,
                StripeTransactionId = "test_stripe_id",
                Amount = 2000
            };

            await CreatePaymentHistory(parserDto);

            db.ChangeTracker.Clear();

            var paymentHistory = await db.PaymentHistories
                .Include(ph => ph.Subscription)
                .Where(ph => ph.UserId == user.Id)
                .ToListAsync();

            paymentHistory.Should().HaveCount(1);
            paymentHistory[0].Amount.Should().Be(2000);

            paymentHistory[0].Subscription.Should().NotBeNull();

            var userUpdated = await db.Users
                .Include(u => u.PaymentHistories)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            userUpdated.HasActiveSubscription.Should().Be(true);

            TestAuthHandler.TestUserId = null;
        }

        [Fact]
        public async Task StripeWebhookHandler_ShouldProcessProductCheckoutSessionCompleted_WithCoupon_AndSavePaymentHistory()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var user = new User($"user_{Guid.NewGuid():N}@test.com", "hashedpass", RoleEnum.DefaultUser);
            db.Users.Add(user);

            TestAuthHandler.TestUserId = user.Id;

            var category = new Category($"Test Category For {Guid.NewGuid():N}");
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            var product = new Product(
                stripeProductId: "product_test_123",
                stripePriceId: "price_test_123",
                name: "TestProduct",
                description: "test descr",
                price: 2000,
                categoryId: category.Id,
                imgUrls: new List<string> { "https://example.com/image.jpg" },
                paragraphs: new List<string> { "Paragraph 1" },
                cardDescription: "Card description"
            );

            db.Products.Add(product);

            var coupon = new Coupon($"{Guid.NewGuid():N}_coupon", DiscountTypeEnum.FixedAmount, 200, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));
            db.Coupons.Add(coupon);

            await db.SaveChangesAsync();

            var parserDto = new WebhookParserDto()
            {
                UserId = user.Id,
                ProductId = product.Id,
                StripeTransactionId = "test_stripe_id",
                Amount = 1800
            };

            await CreatePaymentHistory(
                parserDto,
                new(
                    new() { coupon.Code },
                    new() { "testStipeCouponeId" }
                ));

            db.ChangeTracker.Clear();

            var paymentHistory = await db.PaymentHistories
                .Include(ph => ph.Products)
                .Where(ph => ph.UserId == user.Id)
                .ToListAsync();

            paymentHistory.Should().HaveCount(1);
            paymentHistory[0].Amount.Should().Be(1800);

            var productsPaymentHistory = paymentHistory[0].Products;

            productsPaymentHistory.Should().HaveCount(1);

            TestAuthHandler.TestUserId = null;
        }
    }
}
