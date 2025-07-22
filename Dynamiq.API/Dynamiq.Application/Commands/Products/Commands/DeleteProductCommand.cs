using MediatR;

namespace Dynamiq.Application.Commands.Products.Commands
{
    public record class DeleteProductCommand(Guid Id) : IRequest;
}
