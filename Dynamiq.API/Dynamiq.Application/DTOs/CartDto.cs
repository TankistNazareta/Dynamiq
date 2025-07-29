namespace Dynamiq.Application.DTOs
{
    public record class CartDto(Guid Id, List<CartItemDto> Items);
}
