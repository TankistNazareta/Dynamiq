using Dynamiq.Application.DTOs;
using MediatR;

namespace Dynamiq.Application.Queries.Products.Queries
{
    public record class GetAllProductBySlugQuery(string Slug) : IRequest<IReadOnlyList<ProductDto>>;
}
