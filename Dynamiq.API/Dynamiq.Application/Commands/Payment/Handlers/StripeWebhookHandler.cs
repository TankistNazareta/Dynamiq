using Dynamiq.Application.Commands.Payment.Commands;
using Dynamiq.Application.IntegrationEvents;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Enums;
using Dynamiq.Domain.Exceptions;
using Dynamiq.Domain.Interfaces.Repositories;
using MediatR;

namespace Dynamiq.Application.Commands.Payment.Handlers
{
    public class StripeWebhookHandler : IRequestHandler<StripeWebhookCommand>
    {
        private readonly IPaymentHistoryRepo _repo;
        private readonly ICartRepo _cartRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStripeWebhookParser _webhookParser;
        private readonly IMediator _mediator;

        public StripeWebhookHandler(IPaymentHistoryRepo repo, IUnitOfWork unitOfWork,
            IStripeWebhookParser parser, IMediator mediator, ICartRepo cartRepo)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _webhookParser = parser;
            _mediator = mediator;
            _cartRepo = cartRepo;
        }

        public async Task Handle(StripeWebhookCommand request, CancellationToken cancellationToken)
        {
            var parserDto = _webhookParser.ParseCheckoutSessionCompleted(request.Json, request.Signature);

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

                foreach(var item in cart.Items)
                {
                    paymentHistory.AddProduct(new(item.ProductId, paymentHistory.Id, item.Quantity));
                }

                _cartRepo.Remove(cart);
            }

            await _repo.AddAsync(paymentHistory, cancellationToken);

            if (parserDto.Interval != IntervalEnum.OneTime && parserDto.ProductId != null)
            {
                await _mediator.Publish(new SubscriptionPaymentEvent(
                    paymentHistory.Id, paymentHistory.UserId, parserDto.ProductId.Value, paymentHistory.Interval
                ));
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
