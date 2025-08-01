using Dynamiq.Application.Commands.Payment.Commands;
using Dynamiq.Application.DTOs;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Domain.Enums;
using Dynamiq.Domain.Exceptions;
using Dynamiq.Domain.Interfaces.Repositories;
using MediatR;

namespace Dynamiq.Application.Commands.Payment.Handlers
{
    public class CreateCheckoutSessionHandler : IRequestHandler<CreateCheckoutSessionCommand, string>
    {
        private readonly IStripeCheckoutSession _paymentService;
        private readonly IProductRepo _productRepo;
        private readonly ICartRepo _cartRepo;

        public CreateCheckoutSessionHandler(IStripeCheckoutSession paymentService, IProductRepo productRepo, ICartRepo cartRepo)
        {
            _productRepo = productRepo;
            _paymentService = paymentService;
            _cartRepo = cartRepo;
        }

        public async Task<string> Handle(CreateCheckoutSessionCommand request, CancellationToken cancellationToken)
        {
            var stripeCartItems = new List<StripeCartItemDto>();
            IntervalEnum interval;

            if (request.ProductId != null)
            {
                var product = await _productRepo.GetByIdAsync(request.ProductId.Value, cancellationToken)
                    ?? throw new KeyNotFoundException($"Product with id {request.ProductId} doesn't exist");

                var quantity = request.Quantity ?? 1;
                stripeCartItems.Add(new StripeCartItemDto(product.Id, quantity, product.StripePriceId));
                interval = product.Interval;
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

                    stripeCartItems.Add(new StripeCartItemDto(product.Id, item.Quantity, product.StripePriceId));
                }

                interval = IntervalEnum.OneTime;
            }

            var checkoutSessionRequest = new CheckoutSessionDto
            {
                UserId = request.UserId,
                SuccessUrl = request.SuccessUrl,
                CancelUrl = request.CancelUrl,
                Interval = interval,
                ProductId = request.ProductId
            };

            return await _paymentService.CreateCheckoutSessionAsync(checkoutSessionRequest, stripeCartItems);
        }
    }
}
