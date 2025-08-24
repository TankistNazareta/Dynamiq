namespace Dynamiq.Application.DTOs.ProductDTOs
{
    public record class ResponseProductsDto(int TotalCount, IReadOnlyList<ProductDto> Products);

}
