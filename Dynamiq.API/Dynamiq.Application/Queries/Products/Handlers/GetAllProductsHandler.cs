using AutoMapper;
using Dynamiq.Application.DTOs.ProductDTOs;
using Dynamiq.Application.Queries.Products.Queries;
using Dynamiq.Domain.Interfaces.Repositories;
using MediatR;

namespace Dynamiq.Application.Queries.Products.Handlers
{
    public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, IReadOnlyList<ProductDto>>
    {
        private readonly IProductRepo _repo;
        private readonly IMapper _mapper;

        public GetAllProductsHandler(IProductRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _repo.GetAllAsync(cancellationToken);

            return _mapper.Map<IReadOnlyList<ProductDto>>(products);
        }
    }
}
