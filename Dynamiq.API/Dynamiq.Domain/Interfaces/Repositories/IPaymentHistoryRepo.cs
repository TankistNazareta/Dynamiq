using Dynamiq.Domain.Entities;

namespace Dynamiq.Domain.Interfaces.Repositories
{
    public interface IPaymentHistoryRepo
    {
        Task AddAsync(PaymentHistory paymentHistory, CancellationToken ct);
        Task<IReadOnlyList<PaymentHistory>> GetListByProductIdAsync(Guid productId, CancellationToken ct);
        Task<IReadOnlyList<PaymentHistory>> GetListByUserIdAsync(Guid userId, CancellationToken ct);
    }
}
