using Dynamiq.Application.DTOs.ProductDTOs;
using MediatR;

namespace Dynamiq.Application.Queries.Products.Queries
{
    public record class GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;
}
