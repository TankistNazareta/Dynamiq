using Dynamiq.API.Extension.RequestEntity;
using Dynamiq.API.Mapping.DTOs;
using MediatR;

namespace Dynamiq.API.Commands.Product
{
    public record CreateProductCommand(ProductRequestEntity ProductRequest) : IRequest<ProductDto>;
}
