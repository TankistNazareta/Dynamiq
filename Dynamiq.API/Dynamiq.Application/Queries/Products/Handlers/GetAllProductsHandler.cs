using AutoMapper;
using Dynamiq.Application.DTOs.ProductDTOs;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Application.Queries.Products.Queries;
using MediatR;

namespace Dynamiq.Application.Queries.Products.Handlers
{
    public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, ResponseProductsDto>
    {
        private readonly IProductRepo _repo;
        private readonly IMapper _mapper;

        public GetAllProductsHandler(IProductRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ResponseProductsDto> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _repo.GetAllAsync(request.Limit, request.Offset, cancellationToken);

            return _mapper.Map<ResponseProductsDto>(products);
        }
    }
}
