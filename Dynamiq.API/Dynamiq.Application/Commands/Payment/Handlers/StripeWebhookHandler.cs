using Dynamiq.Application.Commands.Payment.Commands;
using Dynamiq.Application.DTOs.StripeDTOs;
using Dynamiq.Application.IntegrationEvents;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Enums;
using Dynamiq.Domain.Exceptions;
using MediatR;

namespace Dynamiq.Application.Commands.Payment.Handlers
{
    public class StripeWebhookHandler : IRequestHandler<StripeWebhookCommand, bool>
    {
        private readonly IPaymentHistoryRepo _repo;
        private readonly ICartRepo _cartRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStripeWebhookParser _webhookParser;
        private readonly IMediator _mediator;
        private readonly IStripeCouponService _stripeCouponService;
        private readonly ICouponRepo _couponRepo;

        public StripeWebhookHandler(IPaymentHistoryRepo repo, IUnitOfWork unitOfWork,
            IStripeWebhookParser parser, IMediator mediator,
            ICartRepo cartRepo, IStripeCouponService stripeCouponService,
            ICouponRepo couponRepo)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _webhookParser = parser;
            _mediator = mediator;
            _cartRepo = cartRepo;
            _stripeCouponService = stripeCouponService;
            _couponRepo = couponRepo;
        }

        public async Task<bool> Handle(StripeWebhookCommand request, CancellationToken cancellationToken)
        {
            var parserDto = _webhookParser.ParseCheckoutSessionCompleted(request.Json, request.Signature, out var eventType);
            CouponsResultDto? couponsResultDto = null;

            //need to delete stripe discount
            if (eventType == "checkout.session.completed" ||
                eventType == "payment_intent.payment_failed" ||
                eventType == "invoice.payment_failed")
            {
                couponsResultDto = _webhookParser.TryGetCoupons(request.Json, request.Signature);
                if (couponsResultDto != null &&
                    couponsResultDto.CouponsCodeList != null &&
                    couponsResultDto.CouponsCodeList.Count != 0 &&
                    couponsResultDto.StripeCouponIdList != null &&
                    couponsResultDto.StripeCouponIdList.Count != 0)
                {
                    foreach (var id in couponsResultDto.StripeCouponIdList)
                    {
                        await _stripeCouponService.DeactivateCoupon(id);
                    }
                }
            }

            if (parserDto == null)
                return false;

            var paymentHistory = new PaymentHistory(parserDto.UserId, parserDto.StripePaymentId,
                parserDto.Amount, parserDto.Interval);

            if (parserDto.ProductId != null)
            {

                paymentHistory.AddProduct(new(parserDto.ProductId.Value, paymentHistory.Id));
            }
            else
            {
                var cart = await _cartRepo.GetByUserIdAsync(parserDto.UserId, cancellationToken);

                if (cart?.Items == null || cart.Items.Count == 0)
                    throw new CartEmptyException();

                foreach (var item in cart.Items)
                {
                    paymentHistory.AddProduct(new(item.ProductId, paymentHistory.Id, item.Quantity));
                }

                _cartRepo.Remove(cart);
            }

            await _repo.AddAsync(paymentHistory, cancellationToken);

            if (couponsResultDto != null &&
                couponsResultDto.CouponsCodeList != null &&
                couponsResultDto.CouponsCodeList.Count != 0 &&
                couponsResultDto.StripeCouponIdList != null &&
                couponsResultDto.StripeCouponIdList.Count != 0)
            {
                foreach (var id in couponsResultDto.CouponsCodeList)
                {
                    var coupon = await _couponRepo.GetByCodeAsync(id, cancellationToken);
                    coupon.DeactivateCoupon();
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (parserDto.Interval != IntervalEnum.OneTime && parserDto.ProductId != null)
            {
                await _mediator.Publish(new SubscriptionPaymentEvent(
                    paymentHistory.Id, paymentHistory.UserId, parserDto.ProductId.Value, paymentHistory.Interval
                ));

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return true;
        }
    }
}
