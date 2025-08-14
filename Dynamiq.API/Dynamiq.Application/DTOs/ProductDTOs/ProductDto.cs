using Dynamiq.Domain.Enums;

namespace Dynamiq.Application.DTOs.ProductDTOs
{
    public record class ProductDto(Guid Id, string Name,
            string Description, int Price, IntervalEnum Interval);
}
