using Dynamiq.Application.Commands.Products.Commands;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Domain.Interfaces.Repositories;
using MediatR;

namespace Dynamiq.Application.Commands.Products.Handlers
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IProductRepo _repo;
        private readonly IStripeProductService _stripeService;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductHandler(IProductRepo repo, IStripeProductService stripeService, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _stripeService = stripeService;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _repo.GetByIdAsync(request.Id, cancellationToken);

            if (product == null)
                throw new KeyNotFoundException($"product with id: {request.Id} doesn't exist");

            await _stripeService.DeleteProductStripeAsync(product.StripePriceId, product.StripeProductId);

            _repo.Delete(product);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
