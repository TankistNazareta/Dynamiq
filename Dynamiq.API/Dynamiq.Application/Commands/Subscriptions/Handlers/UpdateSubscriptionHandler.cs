using Dynamiq.Application.Commands.Subscriptions.Commands;
using Dynamiq.Application.DTOs.ProductDTOs;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Domain.Aggregates;
using MediatR;

namespace Dynamiq.Application.Commands.Subscriptions.Handlers
{
    public class UpdateSubscriptionHandler : IRequestHandler<UpdateSubscriptionCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubscriptionRepo _repo;
        private readonly IStripeProductService _stripeProductService;

        public UpdateSubscriptionHandler(IUnitOfWork unitOfWork, ISubscriptionRepo repo, IStripeProductService stripeProductService)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _stripeProductService = stripeProductService;   
        }

        public async Task Handle(UpdateSubscriptionCommand request, CancellationToken cancellationToken)
        {
            var subscription = await _repo.GetByIdAsync(request.Id, cancellationToken);

            if (subscription == null)
                throw new KeyNotFoundException($"subscription with id: {request.Id}, wasn't found");

            var productDto = new ProductDto(
                request.Id,
                request.Name,
                "subscription",
                request.Price,
                null,
                null,
                null,
                Guid.Empty
            );

            var stripeIds = await _stripeProductService.UpdateProductStripeAsync(
                productDto,
                subscription.StripeProductId,
                subscription.StripePriceId, 
                request.Interval);

            subscription.Update(request.Name, request.Interval, request.Price, stripeIds.PriceId, stripeIds.ProductId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
