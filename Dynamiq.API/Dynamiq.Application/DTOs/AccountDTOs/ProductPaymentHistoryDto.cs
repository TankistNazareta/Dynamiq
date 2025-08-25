namespace Dynamiq.Application.DTOs.AccountDTOs
{
    public record ProductPaymentHistoryDto
    {
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
    }
}
