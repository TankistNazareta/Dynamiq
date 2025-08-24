using Dynamiq.Application.Commands.Payment.Commands;
using Dynamiq.Application.DTOs.StripeDTOs;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Application.Interfaces.Services;
using Dynamiq.Application.Interfaces.Stripe;
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
        private readonly ICouponService _couponService;
        private readonly ICouponRepo _couponRepo;

        public CreateCheckoutSessionHandler(
            IStripeCheckoutSession paymentService,
            IProductRepo productRepo,
            ICartRepo cartRepo,
            ICouponService couponService,
            ICouponRepo couponRepo)
        {
            _productRepo = productRepo;
            _paymentService = paymentService;
            _cartRepo = cartRepo;
            _couponService = couponService;
            _couponRepo = couponRepo;
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
                stripeCartItems.Add(new StripeCartItemDto(product.Id, quantity, product.StripePriceId, product.Price));
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

                    stripeCartItems.Add(new StripeCartItemDto(product.Id, item.Quantity, product.StripePriceId, product.Price));
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

            if (request.CouponCodes != null && request.CouponCodes.Count > 0)
            {
                foreach (var couponCode in request.CouponCodes)
                {
                    var coupon = await _couponRepo.GetByCodeAsync(couponCode, cancellationToken);

                    if (coupon == null)
                        throw new KeyNotFoundException("coupon wasn`t found: " + couponCode);

                    if (!coupon.IsActive())
                        throw new TimeoutException("Verification token expired.");
                }
                stripeCartItems = await _couponService.AddAllCouponsAsync(stripeCartItems, request.CouponCodes, cancellationToken);
            }

            return await _paymentService.CreateCheckoutSessionAsync(checkoutSessionRequest, stripeCartItems, request.CouponCodes);
        }
    }
}
