namespace Dynamiq.Domain.Common
{
    public class ProductFilter
    {
        public Guid? CategoryId { get; init; }
        public int? MinPrice { get; init; }
        public int? MaxPrice { get; init; }
        public string? SearchTerm { get; init; }
    }
}
