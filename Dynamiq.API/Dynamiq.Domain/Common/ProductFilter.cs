using Dynamiq.Domain.Enums;

namespace Dynamiq.Domain.Common
{
    public class ProductFilter
    {
        public List<Guid>? CategoryIds { get; init; }
        public int? MinPrice { get; init; }
        public int? MaxPrice { get; init; }
        public string? SearchTerm { get; init; }
        public SortEnum? SortBy { get; init; }
    }
}
