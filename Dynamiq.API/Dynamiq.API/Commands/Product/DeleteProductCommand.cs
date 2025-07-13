using MediatR;

namespace Dynamiq.API.Commands.Product
{
    public record DeleteProductCommand(Guid Id) : IRequest;
}
