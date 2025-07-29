using AutoMapper;
using Dynamiq.Application.DTOs;
using Dynamiq.Application.Queries.Users.Queries;
using Dynamiq.Domain.Interfaces.Repositories;
using MediatR;

namespace Dynamiq.Application.Queries.Users.Handlers
{
    internal class GetFilteredProductsHandler : IRequestHandler<GetFilteredProductsQuery, List<ProductDto>>
    {
        private readonly IProductRepo _productRepo;
        private readonly IMapper _mapper;

        public GetFilteredProductsHandler(IProductRepo productRepo, IMapper mapper)
        {
            _productRepo = productRepo;
            _mapper = mapper;
        }

        public async Task<List<ProductDto>> Handle(GetFilteredProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepo.GetFilteredAsync(request.Filter, cancellationToken);
            return _mapper.Map<List<ProductDto>>(products);
        }
    }
}
