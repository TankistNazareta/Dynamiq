using MediatR;

namespace Dynamiq.Application.Commands.Carts.Commands
{
    public record class ClearCartCommand(Guid UserId) : IRequest;
}
