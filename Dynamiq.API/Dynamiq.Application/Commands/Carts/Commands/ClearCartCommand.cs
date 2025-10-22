using MediatR;

namespace Dynamiq.Application.Commands.Carts.Commands
{
    public record class ClearCartCommand() : IRequest
    {
        public Guid UserId { get; set; }
    }
}
