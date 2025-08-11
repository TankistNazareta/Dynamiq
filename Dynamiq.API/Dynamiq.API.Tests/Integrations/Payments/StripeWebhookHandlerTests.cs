using Dynamiq.Application.Commands.Payment.Commands;
using Dynamiq.Application.Commands.Payment.Handlers;
using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Enums;
using Dynamiq.Domain.Interfaces.Repositories;
using Dynamiq.Infrastructure.Persistence.Context;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Dynamiq.API.Tests.Integrations.Payments
{
    public class StripeWebhookHandlerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public StripeWebhookHandlerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        //[Fact]
        //public async Task StripeWebhookHandler_ShouldProcessCheckoutSessionCompleted_AndSavePaymentHistory()
        //{
        //    using var scope = _factory.Services.CreateScope();
        //    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        //    var paymentHistoryRepo = scope.ServiceProvider.GetRequiredService<IPaymentHistoryRepo>();
        //    var cartRepo = scope.ServiceProvider.GetRequiredService<ICartRepo>();
        //    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        //    var user = new User("testuser@example.com", "hashedpass", RoleEnum.DefaultUser);
        //    db.Users.Add(user);

        //    var category = new Category("Test Category");
        //    db.Categories.Add(category);

        //    var product = new Product("product_test_123", "price_test_123", "TestProduct", "test descr", 2000, IntervalEnum.OneTime, category.Id);
        //    db.Products.Add(product);

        //    var cart = new Cart(user.Id);
        //    cart.AddItem(product.Id, 2);
        //    db.Carts.Add(cart);

        //    await db.SaveChangesAsync();

        //    var fakePayload = new
        //    {
        //        data = new
        //        {
        //            obj = new
        //            {
        //                id = "cs_test_123",
        //                client_reference_id = user.Id.ToString(),
        //                amount_total = 2000,
        //                metadata = new
        //                {
        //                    productId = product.Id.ToString()
        //                }
        //            }
        //        },
        //        type = "checkout.session.completed"
        //    };

        //    var jsonPayload = JsonConvert.SerializeObject(fakePayload);

        //    var fakeSignature = "tst_signature";

        //    var handler = scope.ServiceProvider.GetRequiredService<StripeWebhookHandler>();

        //    var command = new StripeWebhookCommand(jsonPayload, fakeSignature);

        //    var result = await handler.Handle(command, default);

        //    result.Should().BeTrue();

        //    var paymentHistories = await paymentHistoryRepo.GetAllAsync();
        //    paymentHistories.Should().ContainSingle(ph =>
        //        ph.UserId == user.Id &&
        //        ph.StripePaymentId == "cs_test_123" &&
        //        ph.Amount == 2000);

        //    var cartAfter = await cartRepo.GetByUserIdAsync(user.Id, default);
        //    cartAfter.Should().BeNull();
        //}
    }
}
