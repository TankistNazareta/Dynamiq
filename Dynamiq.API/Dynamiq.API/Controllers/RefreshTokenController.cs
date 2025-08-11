using Dynamiq.Application.Commands.RefreshTokens.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dynamiq.API.Controllers
{
    [Route("token")]
    [ApiController]
    public class RefreshTokenController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public RefreshTokenController(IMediator mediator, ILogger<RefreshTokenController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPut("revoke")]
        public async Task<IActionResult> Revoke([FromBody] RevokeRefreshTokenCommand command)
        {
            await _mediator.Send(command);

            _logger.LogInformation("Refresh token revoked: {Token}", command.Token);

            return Ok("Token successfully revoked");
        }

        [HttpPut("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTheTokenCommand command)
        {
            var res = await _mediator.Send(command);

            _logger.LogInformation($"Refreshed token with id: {command.RefreshToken}");

            return Ok(res);
        }
    }
}
