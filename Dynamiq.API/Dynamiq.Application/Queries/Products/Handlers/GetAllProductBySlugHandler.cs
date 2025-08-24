using AutoMapper;
using Dynamiq.Application.DTOs.ProductDTOs;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Application.Queries.Products.Queries;
using MediatR;

namespace Dynamiq.Application.Queries.Products.Handlers
{
    public class GetAllProductBySlugHandler : IRequestHandler<GetAllProductBySlugQuery, ResponseProductsDto>
    {
        private readonly IProductRepo _repo;
        private readonly IMapper _mapper;

        public GetAllProductBySlugHandler(IProductRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ResponseProductsDto> Handle(GetAllProductBySlugQuery request, CancellationToken cancellationToken)
        {
            var products = await _repo.GetAllBySlugAsync(request.Slug, request.Limit, request.Offset, cancellationToken);

            return _mapper.Map<ResponseProductsDto>(products);
        }
    }
}
