using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Application.Queries.Products.Queries;
using MediatR;

namespace Dynamiq.Application.Queries.Products.Handlers
{
    public class GetSearchedProductHandler : IRequestHandler<GetSearchedProductQuery, List<string>>
    {
        private readonly IProductRepo _productRepo;

        public GetSearchedProductHandler(IProductRepo productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<List<string>> Handle(GetSearchedProductQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepo.GetFilteredNamesAsync(request.ProductFilter, request.Limit, cancellationToken);

            return products;
        }
    }
}
