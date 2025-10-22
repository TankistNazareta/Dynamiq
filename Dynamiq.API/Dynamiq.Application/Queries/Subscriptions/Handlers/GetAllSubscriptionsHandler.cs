using AutoMapper;
using Dynamiq.Application.DTOs.AccountDTOs;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Application.Queries.Subscriptions.Queries;
using MediatR;

namespace Dynamiq.Application.Queries.Subscriptions.Handlers
{
    public class GetAllSubscriptionsHandler : IRequestHandler<GetAllSubscriptionsQuery, List<SubscriptionDto>>
    {
        private readonly ISubscriptionRepo _repo;
        private readonly IMapper _mapper;

        public GetAllSubscriptionsHandler(ISubscriptionRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<SubscriptionDto>> Handle(GetAllSubscriptionsQuery request, CancellationToken cancellationToken)
        {
            var items = await _repo.GetAllAsync(cancellationToken);

            return _mapper.Map<List<SubscriptionDto>>(items);
        }
    }
}
