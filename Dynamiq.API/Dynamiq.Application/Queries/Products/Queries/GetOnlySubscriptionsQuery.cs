using Dynamiq.Application.DTOs.ProductDTOs;
using MediatR;

namespace Dynamiq.Application.Queries.Products.Queries
{
    public record class GetOnlySubscriptionsQuery() : IRequest<List<ProductDto>>;
}
