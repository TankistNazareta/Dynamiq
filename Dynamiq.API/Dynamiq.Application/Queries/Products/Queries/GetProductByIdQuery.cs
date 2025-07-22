using Dynamiq.Application.DTOs;
using MediatR;

namespace Dynamiq.Application.Queries.Products.Queries
{
    public record class GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;
}
