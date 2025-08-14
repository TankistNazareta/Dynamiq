using Dynamiq.Application.DTOs.ProductDTOs;
using MediatR;

namespace Dynamiq.Application.Queries.Products.Queries
{
    public class GetAllProductsQuery : IRequest<IReadOnlyList<ProductDto>>;
}
