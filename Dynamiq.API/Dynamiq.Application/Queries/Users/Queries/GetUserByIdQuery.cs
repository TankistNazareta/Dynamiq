using Dynamiq.Application.DTOs.AccountDTOs;
using MediatR;

namespace Dynamiq.Application.Queries.Users.Queries
{
    public record GetUserByIdQuery(Guid Id) : IRequest<UserDto>;
}
