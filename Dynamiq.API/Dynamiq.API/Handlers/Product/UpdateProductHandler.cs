using Dynamiq.API.Commands.Product;
using Dynamiq.API.Interfaces;
using Dynamiq.API.Mapping.DTOs;
using Dynamiq.API.Stripe.Interfaces;
using Dynamiq.API.Stripe.Services;
using MediatR;

namespace Dynamiq.API.Handlers.Product
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, ProductDto>
    {
        private readonly IStripeProductService _stripeService;
        private readonly IProductRepo _repo;

        public UpdateProductHandler(IStripeProductService stripeService, IProductRepo repo)
        {
            _stripeService = stripeService;
            _repo = repo;
        }

        public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var updated = await _stripeService.UpdateProductStripe(request.Product);
            var res = await _repo.Update(updated);

            return res;
        }
    }
}
