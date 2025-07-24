namespace Dynamiq.Application.DTOs
{
    public record class CategoryDto(Guid Id, string Name, string Slug, List<CategoryDto> SubCategories);
}
