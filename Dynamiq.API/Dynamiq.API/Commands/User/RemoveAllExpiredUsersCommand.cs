using MediatR;

namespace Dynamiq.API.Commands.User
{
    public record RemoveAllExpiredUsersCommand() : IRequest<int>;
}
