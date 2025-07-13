using Dynamiq.API.Commands.Product;
using Dynamiq.API.Interfaces;
using Dynamiq.API.Stripe.Interfaces;
using Dynamiq.API.Stripe.Services;
using MediatR;

namespace Dynamiq.API.Handlers.Product
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IStripeProductService _stripeService;
        private readonly IProductRepo _repo;

        public DeleteProductHandler(IStripeProductService stripeService, IProductRepo repo)
        {
            _stripeService = stripeService;
            _repo = repo;
        }

        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _repo.GetById(request.Id);

            await _repo.Delete(request.Id);
            await _stripeService.DeleteProductStripe(product.StripePriceId, product.StripeProductId);
        }
    }
}
