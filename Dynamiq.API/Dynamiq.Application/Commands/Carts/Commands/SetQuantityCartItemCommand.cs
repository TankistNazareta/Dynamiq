using Dynamiq.Application.DTOs.AccountDTOs;
using MediatR;

namespace Dynamiq.Application.Commands.Carts.Commands
{
    public record class SetQuantityCartItemCommand(Guid ProductId, int Quantity) : IRequest<CartDto>
    {
        public Guid UserId { get; set; }
    }
}
