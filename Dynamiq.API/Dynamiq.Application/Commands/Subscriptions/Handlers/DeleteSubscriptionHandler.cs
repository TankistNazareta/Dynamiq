using Dynamiq.Application.Commands.Subscriptions.Commands;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Application.Interfaces.Stripe;
using MediatR;

namespace Dynamiq.Application.Commands.Subscriptions.Handlers
{
    public class DeleteSubscriptionHandler : IRequestHandler<DeleteSubscriptionCommand>
    {
        private readonly ISubscriptionRepo _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStripeProductService _stripeProductService;

        public DeleteSubscriptionHandler(ISubscriptionRepo repo, IUnitOfWork unitOfWork, IStripeProductService stripeProductService)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _stripeProductService = stripeProductService;
        }

        public async Task Handle(DeleteSubscriptionCommand request, CancellationToken cancellationToken)
        {
            var subscription = await _repo.GetByIdAsync(request.SubscriptionId, cancellationToken);

            if(subscription == null)
                throw new KeyNotFoundException("Subscription not found");
            
            await _stripeProductService.DeleteProductStripeAsync(subscription.StripePriceId, subscription.StripeProductId);

            _repo.Delete(subscription);
        }
    }
}
