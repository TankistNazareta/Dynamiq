namespace Dynamiq.Application.DTOs.StripeDTOs
{
    public record class StripeCartItemDto(Guid ProductId, int Quantity, string StripePriceId);
}
