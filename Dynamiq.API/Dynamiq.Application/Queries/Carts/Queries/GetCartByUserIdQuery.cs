using Dynamiq.Application.DTOs;
using MediatR;

namespace Dynamiq.Application.Queries.Carts.Queries
{
    public record class GetCartByUserIdQuery(Guid UserId) : IRequest<CartDto?>;
}
