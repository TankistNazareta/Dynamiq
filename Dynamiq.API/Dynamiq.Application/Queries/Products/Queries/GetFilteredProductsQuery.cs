using Dynamiq.Application.DTOs;
using Dynamiq.Domain.Common;
using MediatR;

namespace Dynamiq.Application.Queries.Products.Queries
{
    public record class GetFilteredProductsQuery(ProductFilter Filter) : IRequest<List<ProductDto>>;
}
