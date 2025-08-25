using AutoMapper;
using Dynamiq.Application.DTOs.AccountDTOs;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Application.Queries.PaymentHistory.Queries;
using MediatR;

namespace Dynamiq.Application.Queries.PaymentHistory.Handlers
{
    public class GetPaymentHistoriesByUserIdHandler : IRequestHandler<GetPaymentHistoriesByUserIdQuery, List<PaymentHistoryDto>>
    {
        private readonly IPaymentHistoryRepo _paymentHistoryRepo;
        private readonly IMapper _mapper;

        public GetPaymentHistoriesByUserIdHandler(IPaymentHistoryRepo paymentHistoryRepo, IMapper mapper)
        {
            _paymentHistoryRepo = paymentHistoryRepo;
            _mapper = mapper;
        }

        public async Task<List<PaymentHistoryDto>> Handle(GetPaymentHistoriesByUserIdQuery request, CancellationToken cancellationToken)
        {
            var paymentHistories = await _paymentHistoryRepo.GetListByUserIdAsync(request.UserId, cancellationToken);

            return _mapper.Map<List<PaymentHistoryDto>>(paymentHistories);
        }
    }
}
