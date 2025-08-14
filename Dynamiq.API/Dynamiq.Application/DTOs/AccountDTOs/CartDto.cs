namespace Dynamiq.Application.DTOs.AccountDTOs
{
    public record class CartDto(Guid Id, List<CartItemDto> Items);
}
