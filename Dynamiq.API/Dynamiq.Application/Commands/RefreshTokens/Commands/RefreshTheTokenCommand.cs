using Dynamiq.Application.DTOs.AuthDTOs;
using MediatR;

namespace Dynamiq.Application.Commands.RefreshTokens.Commands
{
    public record class RefreshTheTokenCommand(string RefreshToken) : IRequest<AuthResponseDto>;
}
