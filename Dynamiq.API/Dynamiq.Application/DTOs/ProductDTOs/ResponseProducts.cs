using Dynamiq.Domain.Aggregates;

namespace Dynamiq.Application.DTOs.ProductDTOs
{
    public record class ResponseProducts(int TotalCount, IReadOnlyList<Product> Products);
}
