using Dynamiq.Application.DTOs.AccountDTOs;
using MediatR;

namespace Dynamiq.Application.Queries.PaymentHistory.Queries
{
    public record class GetPaymentHistoriesByUserEmailQuery(string UserEmail) : IRequest<List<PaymentHistoryDto>>;
}
