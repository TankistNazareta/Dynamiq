using Dynamiq.Application.Commands.Subscriptions.Command;
using Dynamiq.Application.DTOs.ProductDTOs;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Domain.Aggregates;
using MediatR;

namespace Dynamiq.Application.Commands.Subscriptions.Handler
{
    public class AddSubscriptionHandler : IRequestHandler<AddSubscriptionCommand>
    {
        private readonly ISubscriptionRepo _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStripeProductService _stripeProductService;

        public AddSubscriptionHandler(ISubscriptionRepo repo, IUnitOfWork unitOfWork, IStripeProductService stripeProductService)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _stripeProductService = stripeProductService;
        }

        public async Task Handle(AddSubscriptionCommand request, CancellationToken cancellationToken)
        {
            var productDto = new ProductDto(
                Guid.Empty, request.Name,
                "subscription", request.Price,
                null, null, null, Guid.Empty);

            var stripeIds = await _stripeProductService.CreateProductStripeAsync(productDto, request.Interval);

            var subscription = new Subscription(

                productDto.Name,
                request.Interval,
                productDto.Price,
                stripeIds.PriceId,
                stripeIds.ProductId
            );

            await _repo.AddAsync(subscription, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
