using AutoMapper;
using Dynamiq.Application.DTOs.ProductDTOs;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Application.Queries.Products.Queries;
using MediatR;

namespace Dynamiq.Application.Queries.Products.Handlers
{
    public class GetOnlySubscriptionsHandler : IRequestHandler<GetOnlySubscriptionsQuery, List<ProductDto>>
    {
        private readonly IProductRepo _repo;
        private readonly IMapper _mapper;

        public GetOnlySubscriptionsHandler(IProductRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<ProductDto>> Handle(GetOnlySubscriptionsQuery request, CancellationToken cancellationToken)
        {
            var products = await _repo.GetOnlySubscriptions(cancellationToken);

            return _mapper.Map<List<ProductDto>>(products);
        }
    }
}
