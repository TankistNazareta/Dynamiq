using Dynamiq.Domain.Enums;

namespace Dynamiq.Application.DTOs.AccountDTOs
{
    public record PaymentHistoryDto
    {
        public Guid Id { get; init; }
        public decimal Amount { get; init; }
        public IntervalEnum Interval { get; init; }
        public DateTime CreatedAt { get; init; }
        public IReadOnlyCollection<ProductPaymentHistoryDto> Products { get; init; }
    }
}
