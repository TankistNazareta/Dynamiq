using Dynamiq.Application.DTOs.AuthDTOs;
using MediatR;

namespace Dynamiq.Application.Commands.GoogleAuth.Commands
{
    public record GoogleCallbackCommand(string Code, string? State) : IRequest<AuthTokensDto>;
}
