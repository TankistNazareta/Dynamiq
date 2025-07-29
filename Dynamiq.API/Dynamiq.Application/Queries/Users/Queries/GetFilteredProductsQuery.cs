using Dynamiq.Application.DTOs;
using Dynamiq.Domain.Common;
using MediatR;

namespace Dynamiq.Application.Queries.Users.Queries
{
    public record class GetFilteredProductsQuery(ProductFilter Filter) : IRequest<List<ProductDto>>;
}
