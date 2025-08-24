using Dynamiq.Domain.Enums;
using Dynamiq.Domain.ValueObject;

namespace Dynamiq.Application.DTOs.ProductDTOs
{
    public record class ProductDto(
        Guid Id, string Name,
        string Description, int Price,
        IntervalEnum Interval, List<ProductImgUrl> ImgUrls,
        List<ProductParagraph> Paragraphs, string CardDescription,
        Guid CategoryId
    );
}
