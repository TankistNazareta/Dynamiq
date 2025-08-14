namespace Dynamiq.Application.DTOs.ProductDTOs
{
    public record class CategoryDto(Guid Id, string Name, string Slug, List<CategoryDto> SubCategories);
}
