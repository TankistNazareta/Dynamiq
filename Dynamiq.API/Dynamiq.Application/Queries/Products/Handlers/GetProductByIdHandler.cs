using AutoMapper;
using Dynamiq.Application.DTOs;
using Dynamiq.Application.Queries.Products.Queries;
using Dynamiq.Domain.Interfaces.Repositories;
using MediatR;

namespace Dynamiq.Application.Queries.Products.Handlers
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
    {
        private readonly IProductRepo _repo;
        private readonly IMapper _mapper;

        public GetProductByIdHandler(IProductRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _repo.GetByIdAsync(request.Id, cancellationToken);

            return _mapper.Map<ProductDto>(product);
        }
    }
}
