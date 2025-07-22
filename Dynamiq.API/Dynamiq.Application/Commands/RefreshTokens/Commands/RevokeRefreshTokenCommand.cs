using MediatR;

namespace Dynamiq.Application.Commands.RefreshTokens.Commands
{
    public record class RevokeRefreshTokenCommand(string Token) : IRequest;
}
