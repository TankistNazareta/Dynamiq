using Dynamiq.Domain.Entities;

namespace Dynamiq.Application.Interfaces.Repositories
{
    public interface IProductPaymentHistoryRepo
    {
        Task AddAsync(ProductPaymentHistory productPaymentHistory, CancellationToken ct);
    }
}
