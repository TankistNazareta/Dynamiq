using Dynamiq.Domain.Aggregates;

namespace Dynamiq.Application.Interfaces.Repositories
{
    public interface IPaymentHistoryRepo
    {
        Task AddAsync(PaymentHistory paymentHistory, CancellationToken ct);
        Task<IReadOnlyList<PaymentHistory>> GetListByUserIdAsync(Guid userId, CancellationToken ct);
    }
}
