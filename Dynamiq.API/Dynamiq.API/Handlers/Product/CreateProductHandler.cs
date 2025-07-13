using Dynamiq.API.Commands.Product;
using Dynamiq.API.Interfaces;
using Dynamiq.API.Mapping.DTOs;
using Dynamiq.API.Stripe.Interfaces;
using MediatR;

namespace Dynamiq.API.Handlers.Product
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly IStripeProductService _stripeService;
        private readonly IProductRepo _repo;

        public CreateProductHandler(IStripeProductService stripeService, IProductRepo repo)
        {
            _stripeService = stripeService;
            _repo = repo;
        }

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var productDto = await _stripeService.CreateProductStripe(request.Product);
            var inserted = await _repo.Insert(productDto);

            return inserted;
        }
    }
}
