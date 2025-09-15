using MediatR;

namespace Dynamiq.Application.Commands.Products.Commands
{
    public record class AddViewCountCommand(Guid Id) : IRequest;
}
