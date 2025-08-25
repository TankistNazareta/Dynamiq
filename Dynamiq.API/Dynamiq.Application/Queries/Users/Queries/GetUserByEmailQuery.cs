using Dynamiq.Application.DTOs.AccountDTOs;
using MediatR;

namespace Dynamiq.Application.Queries.Users.Queries
{
    public record class GetUserByEmailQuery(string Email) : IRequest<UserDto>;
}
