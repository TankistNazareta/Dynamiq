using Dynamiq.Application.DTOs;
using MediatR;

namespace Dynamiq.Application.Commands.RefreshTokens.Commands
{
    public record class RefreshTheTokenCommand(string RefreshToken) : IRequest<AuthResponseDto>;
}
