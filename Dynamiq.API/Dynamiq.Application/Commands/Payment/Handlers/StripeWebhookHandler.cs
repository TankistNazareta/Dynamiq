using Dynamiq.Application.Commands.Payment.Commands;
using Dynamiq.Application.DTOs.PaymentDTOs;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Enums;
using Dynamiq.Domain.Exceptions;
using MediatR;
using System.Threading;

namespace Dynamiq.Application.Commands.Payment.Handlers
{
    public class StripeWebhookHandler : IRequestHandler<StripeWebhookCommand, bool>
    {
        private readonly IPaymentHistoryRepo _repo;
        private readonly ICartRepo _cartRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStripeWebhookParser _webhookParser;
        private readonly IStripeCouponService _stripeCouponService;
        private readonly ICouponRepo _couponRepo;

        public StripeWebhookHandler(IPaymentHistoryRepo repo, IUnitOfWork unitOfWork,
            IStripeWebhookParser parser,
            ICartRepo cartRepo, IStripeCouponService stripeCouponService,
            ICouponRepo couponRepo)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _webhookParser = parser;
            _cartRepo = cartRepo;
            _stripeCouponService = stripeCouponService;
            _couponRepo = couponRepo;
        }

        public async Task<bool> Handle(StripeWebhookCommand request, CancellationToken cancellationToken)
        {
            var eventType = _webhookParser.GetEventType(request.Json, request.Signature);

            switch(eventType) {
                case "checkout.session.completed": 
                    await OnPayment(request, cancellationToken);
                    break;
                case "customer.subscription.deleted":
                    await OnDeactivateSubscription(request, cancellationToken);
                    break;
                default:
                    return false;
            }

            return true;
        }
        
        private async Task OnDeactivateSubscription(StripeWebhookCommand request, CancellationToken cancellationToken)
        {
            var subscriptionId = _webhookParser.ParseDeletedSubscriptionId(request.Json, request.Signature);

            var paymentHistory = await _repo.GetBySubscriptionIdAsync(subscriptionId, cancellationToken);

            if(paymentHistory == null)
                throw new KeyNotFoundException("Payment history with this subscription id wasn't found");

            paymentHistory.Subscription.Cancel();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task OnPayment(StripeWebhookCommand request, CancellationToken cancellationToken)
        {
            var parserDto = _webhookParser.ParseCheckoutSessionCompleted(request.Json, request.Signature);
            CouponsResultDto? couponsResultDto = null;

            //need to delete stripe discount
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
            
            var paymentHistory = new PaymentHistory(parserDto.UserId, parserDto.StripeTransactionId,
                parserDto.Amount);

            if (parserDto.ProductId != null)
            {
                if (parserDto.Interval == null)

                    paymentHistory.AddProduct(parserDto.ProductId.Value);
                else
                    paymentHistory.SetSubscription(parserDto.ProductId.Value);
            }
            else
            {
                var cart = await _cartRepo.GetByUserIdAsync(parserDto.UserId, cancellationToken);

                if (cart?.Items == null || cart.Items.Count == 0)
                    throw new CartEmptyException();

                foreach (var item in cart.Items)
                {
                    paymentHistory.AddProduct(item.ProductId, item.Quantity);
                }

                cart.Clear();
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
        }
    }
}
