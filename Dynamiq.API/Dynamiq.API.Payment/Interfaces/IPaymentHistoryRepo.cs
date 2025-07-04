using Dynamiq.API.Mapping.DTOs;

namespace Dynamiq.API.Stripe.Interfaces
{
    public interface IPaymentHistoryRepo
    {
        Task<List<PaymentHistoryDto>> GetAll();
        Task<PaymentHistoryDto> GetById(Guid id);
        Task<PaymentHistoryDto> Insert(PaymentHistoryDto paymentHistory);
        Task Delete(Guid id);
    }
}
