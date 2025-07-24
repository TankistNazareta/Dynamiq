using AutoMapper;
using Dynamiq.Application.DTOs;
using Dynamiq.Application.Queries.Products.Queries;
using Dynamiq.Domain.Interfaces.Repositories;
using MediatR;

namespace Dynamiq.Application.Queries.Products.Handlers
{
    public class GetAllProductBySlugHandler : IRequestHandler<GetAllProductBySlugQuery, IReadOnlyList<ProductDto>>
    {
        private readonly IProductRepo _repo;
        private readonly IMapper _mapper;

        public GetAllProductBySlugHandler(IProductRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<ProductDto>> Handle(GetAllProductBySlugQuery request, CancellationToken cancellationToken)
        {
            var products = await _repo.GetAllBySlugAsync(request.Slug, cancellationToken);

            return _mapper.Map<IReadOnlyList<ProductDto>>(products);
        }
    }
}
