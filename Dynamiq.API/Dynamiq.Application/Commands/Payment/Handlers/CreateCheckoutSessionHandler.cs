using Dynamiq.Application.Commands.Payment.Commands;
using Dynamiq.Application.DTOs.PaymentDTOs;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Application.Interfaces.Services;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Enums;
using Dynamiq.Domain.Exceptions;
using MediatR;

namespace Dynamiq.Application.Commands.Payment.Handlers
{
    public class CreateCheckoutSessionHandler : IRequestHandler<CreateCheckoutSessionCommand, string>
    {
        private readonly IStripeCheckoutSession _paymentService;
        private readonly IProductRepo _productRepo;
        private readonly ICartRepo _cartRepo;
        private readonly ISubscriptionRepo _subscriptionRepo;
        private readonly IUserRepo _userRepo;

        public CreateCheckoutSessionHandler(
            IStripeCheckoutSession paymentService,
            IProductRepo productRepo,
            ICartRepo cartRepo,
            ISubscriptionRepo subscriptionRepo, IUserRepo userRepo)
        {
            _productRepo = productRepo;
            _paymentService = paymentService;
            _cartRepo = cartRepo;
            _subscriptionRepo = subscriptionRepo;
            _userRepo = userRepo;
        }

        public async Task<string> Handle(CreateCheckoutSessionCommand request, CancellationToken cancellationToken)
        {
            var stripeCartItems = new List<StripeCartItemDto>();
            IntervalEnum? interval = null;

            if (request.ProductId != null)
            {
                var product = await _productRepo.GetByIdAsync(request.ProductId.Value, cancellationToken)
                    ?? throw new KeyNotFoundException($"Product with id {request.ProductId} doesn't exist");

                var quantity = request.Quantity ?? 1;
                stripeCartItems.Add(new StripeCartItemDto(product.Id, quantity, product.StripePriceId, product.Price));
            }
            else if(request.SubscriptionId != null)
            {
                var user = await _userRepo.GetByIdAsync(request.UserId, cancellationToken)
                    ?? throw new KeyNotFoundException($"User with id {request.UserId} doesn't exist");

                if(user.HasActiveSubscription)
                    throw new ActiveSubscriptionExistsException("User already has an active subscription");

                var subscription = await _subscriptionRepo.GetByIdAsync(request.SubscriptionId.Value, cancellationToken)
                    ?? throw new KeyNotFoundException($"Subscription with id {request.SubscriptionId} doesn't exist");

                interval = subscription.Interval;
                
                stripeCartItems.Add(new StripeCartItemDto(subscription.Id, 1, subscription.StripePriceId, subscription.Price));
            }
            else
            {
                var cart = await _cartRepo.GetByUserIdAsync(request.UserId, cancellationToken);

                if (cart?.Items == null || cart.Items.Count == 0)
                    throw new CartEmptyException();

                foreach (var item in cart.Items)
                {
                    var product = await _productRepo.GetByIdAsync(item.ProductId, cancellationToken)
                        ?? throw new KeyNotFoundException($"Product with id {item.ProductId} wasn't found in database, please remove this item from cart and try again");

                    stripeCartItems.Add(new StripeCartItemDto(product.Id, item.Quantity, product.StripePriceId, product.Price));
                }
            }

            var checkoutSessionRequest = new CheckoutSessionDto
            {
                UserId = request.UserId,
                SuccessUrl = request.SuccessUrl,
                CancelUrl = request.CancelUrl,
                Interval = interval,
                ProductId = request.ProductId ?? request.SubscriptionId,
            };

            return await _paymentService.CreateCheckoutSessionAsync(checkoutSessionRequest, stripeCartItems, request.CouponCodes, cancellationToken);
        }
    }
}
