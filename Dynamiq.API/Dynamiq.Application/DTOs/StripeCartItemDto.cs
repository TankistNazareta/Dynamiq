namespace Dynamiq.Application.DTOs
{
    public record class StripeCartItemDto(Guid ProductId, int Quantity, string StripePriceId);
}
