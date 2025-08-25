using AutoMapper;
using Dynamiq.Application.DTOs.AccountDTOs;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Application.Queries.PaymentHistory.Queries;
using MediatR;

namespace Dynamiq.Application.Queries.PaymentHistory.Handlers
{
    public class GetPaymentHistoriesByUserEmailHandler : IRequestHandler<GetPaymentHistoriesByUserEmailQuery, List<PaymentHistoryDto>>
    {
        private readonly IPaymentHistoryRepo _paymentHistoryRepo;
        private readonly IMapper _mapper;
        private readonly IUserRepo _userRepo;

        public GetPaymentHistoriesByUserEmailHandler(IPaymentHistoryRepo paymentHistoryRepo, IMapper mapper, IUserRepo userRepo)
        {
            _paymentHistoryRepo = paymentHistoryRepo;
            _mapper = mapper;
            _userRepo = userRepo;
        }

        public async Task<List<PaymentHistoryDto>> Handle(GetPaymentHistoriesByUserEmailQuery request, CancellationToken cancellationToken)
        {
            var userId = await _userRepo.GetUserIdByHisEmailAsync(request.UserEmail, cancellationToken);

            if (userId == null)
                throw new KeyNotFoundException("user with email wasn't found");

            var paymentHistories = await _paymentHistoryRepo.GetListByUserIdAsync(userId.Value, cancellationToken);

            return _mapper.Map<List<PaymentHistoryDto>>(paymentHistories);
        }
    }
}
