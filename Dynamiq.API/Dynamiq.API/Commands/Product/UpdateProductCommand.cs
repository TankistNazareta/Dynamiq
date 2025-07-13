using Dynamiq.API.Extension.RequestEntity;
using Dynamiq.API.Mapping.DTOs;
using MediatR;

namespace Dynamiq.API.Commands.Product
{
    public record UpdateProductCommand(ProductDto Product) : IRequest<ProductDto>;
}
