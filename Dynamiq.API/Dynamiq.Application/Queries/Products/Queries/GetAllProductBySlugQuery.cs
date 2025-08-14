using Dynamiq.Application.DTOs.ProductDTOs;
using MediatR;

namespace Dynamiq.Application.Queries.Products.Queries
{
    public record class GetAllProductBySlugQuery(string Slug) : IRequest<IReadOnlyList<ProductDto>>;
}
