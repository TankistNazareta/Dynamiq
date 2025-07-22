using Dynamiq.Application.DTOs;
using MediatR;

namespace Dynamiq.Application.Queries.Products.Queries
{
    public class GetAllProductsQuery : IRequest<IReadOnlyList<ProductDto>>;
}
