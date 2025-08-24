using Dynamiq.Application.Commands.Products.Commands;
using Dynamiq.Application.DTOs.ProductDTOs;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Domain.Aggregates;
using MediatR;

namespace Dynamiq.Application.Commands.Products.Handlers
{
    public class AddProductHandler : IRequestHandler<AddProductCommand>
    {
        private readonly IProductRepo _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStripeProductService _stripeService;

        public AddProductHandler(IProductRepo repo, IUnitOfWork unitOfWork, IStripeProductService stripeService)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _stripeService = stripeService;
        }

        public async Task Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            var productDto = new ProductDto(
                Guid.Empty, request.Name,
                request.Description, request.Price,
                request.Interval, null, null, null, Guid.Empty);
            var stripeIds = await _stripeService.CreateProductStripeAsync(productDto);

            var product = new Product(
                stripeIds.ProductId,
                stripeIds.PriceId,
                productDto.Name,
                productDto.Description,
                productDto.Price,
                productDto.Interval,
                request.CategoryId,
                request.ImgUrls,
                request.Paragraphs,
                request.CardDescription
            );

            await _repo.AddAsync(product, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
