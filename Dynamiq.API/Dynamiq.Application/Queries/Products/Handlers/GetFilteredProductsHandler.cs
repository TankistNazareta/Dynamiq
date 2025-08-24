using AutoMapper;
using Dynamiq.Application.DTOs.ProductDTOs;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Application.Queries.Products.Queries;
using MediatR;

namespace Dynamiq.Application.Queries.Products.Handlers
{
    internal class GetFilteredProductsHandler : IRequestHandler<GetFilteredProductsQuery, ResponseProductsDto>
    {
        private readonly IProductRepo _productRepo;
        private readonly IMapper _mapper;

        public GetFilteredProductsHandler(IProductRepo productRepo, IMapper mapper)
        {
            _productRepo = productRepo;
            _mapper = mapper;
        }

        public async Task<ResponseProductsDto> Handle(GetFilteredProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepo.GetFilteredAsync(request.Filter, request.Limit, request.Offset, cancellationToken);
            return _mapper.Map<ResponseProductsDto>(products);
        }
    }
}
