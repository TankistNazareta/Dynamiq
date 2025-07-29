using Dynamiq.Application.DTOs;
using MediatR;

namespace Dynamiq.Application.Commands.Carts.Commands
{
    public record class AddItemToCartCommand(Guid UserId, Guid ProductId, int Quantity) : IRequest<CartDto>;
}
