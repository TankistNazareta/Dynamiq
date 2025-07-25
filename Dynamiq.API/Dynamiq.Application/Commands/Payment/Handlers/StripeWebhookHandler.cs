using Dynamiq.Application.Commands.Payment.Commands;
using Dynamiq.Application.IntegrationEvents;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Enums;
using Dynamiq.Domain.Interfaces.Repositories;
using MediatR;

namespace Dynamiq.Application.Commands.Payment.Handlers
{
    public class StripeWebhookHandler : IRequestHandler<StripeWebhookCommand>
    {
        private readonly IPaymentHistoryRepo _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStripeWebhookParser _webhookParser;
        private readonly IMediator _mediator;

        public StripeWebhookHandler(IPaymentHistoryRepo repo, IUnitOfWork unitOfWork,
            IStripeWebhookParser parser, IMediator mediator)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _webhookParser = parser;
            _mediator = mediator;

        }

        public async Task Handle(StripeWebhookCommand request, CancellationToken cancellationToken)
        {
            var paymentHistoryDto = _webhookParser.ParseCheckoutSessionCompleted(request.Json, request.Signature);

            var paymentHistory = new PaymentHistory(paymentHistoryDto.UserId,
                paymentHistoryDto.ProductId, paymentHistoryDto.StripePaymentId,
                paymentHistoryDto.Amount, paymentHistoryDto.Interval);

            await _repo.AddAsync(paymentHistory, cancellationToken);

            if (paymentHistoryDto.Interval != IntervalEnum.OneTime)
            {
                await _mediator.Publish(new SubscriptionPaymentEvent(
                    paymentHistory.Id, paymentHistory.UserId, paymentHistory.ProductId, paymentHistory.Interval
                ));
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
