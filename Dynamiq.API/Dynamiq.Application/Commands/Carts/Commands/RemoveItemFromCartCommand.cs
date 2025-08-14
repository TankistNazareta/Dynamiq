using Dynamiq.Application.DTOs.AccountDTOs;
using MediatR;

namespace Dynamiq.Application.Commands.Carts.Commands
{
    public record class RemoveItemFromCartCommand(Guid UserId, Guid ProductId, int Quantity) : IRequest<CartDto>;
}
