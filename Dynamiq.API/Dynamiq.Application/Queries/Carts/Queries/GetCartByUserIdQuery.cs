using Dynamiq.Application.DTOs.AccountDTOs;
using MediatR;

namespace Dynamiq.Application.Queries.Carts.Queries
{
    public record class GetCartByUserIdQuery(Guid UserId) : IRequest<CartDto?>;
}
