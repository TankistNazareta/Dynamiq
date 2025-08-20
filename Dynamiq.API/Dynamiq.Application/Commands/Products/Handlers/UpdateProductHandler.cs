using Dynamiq.Application.Commands.Products.Commands;
using Dynamiq.Application.DTOs.ProductDTOs;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Domain.Interfaces.Repositories;
using MediatR;

namespace Dynamiq.Application.Commands.Products.Handlers
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand>
    {
        private readonly IProductRepo _repo;
        private readonly IStripeProductService _stripeService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductHandler(IProductRepo repo, IStripeProductService stripeService, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _stripeService = stripeService;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var productDto = new ProductDto(
                request.Id, 
                request.Name, 
                request.Description, 
                request.Price, 
                request.Interval,
                request.ImgUrl
            );

            var product = await _repo.GetByIdAsync(productDto.Id, cancellationToken);

            if (product == null)
                throw new KeyNotFoundException($"Product with id: {productDto.Id} doesn't exist");

            var stripeIds = await _stripeService.UpdateProductStripeAsync(productDto, product.StripeProductId, product.StripePriceId);

            product.Update(
                stripeIds.ProductId,
                stripeIds.PriceId,
                productDto.Name,
                productDto.Description,
                productDto.Price,
                productDto.Interval,
                request.CategoryId, 
                request.ImgUrl
            );

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
