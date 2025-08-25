using Dynamiq.Application.DTOs.AccountDTOs;
using MediatR;

namespace Dynamiq.Application.Queries.PaymentHistory.Queries
{
    public record class GetPaymentHistoriesByUserIdQuery(Guid UserId) : IRequest<List<PaymentHistoryDto>>;
}
