using Dynamiq.Application.Commands.RefreshTokens.Commands;
using Dynamiq.Domain.Entities;
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
        public async Task<IActionResult> Revoke()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var token))
                return Unauthorized(new { message = "No refresh token" });

            await _mediator.Send(new RevokeRefreshTokenCommand(token));

            Response.Cookies.Delete("refreshToken");

            _logger.LogInformation("Refresh token revoked: {Token}", token);

            return Ok("Token successfully revoked");
        }

        [HttpPut("refresh")]
        public async Task<IActionResult> Refresh()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var token))
                return Unauthorized(new { message = "No refresh token" });

            var res = await _mediator.Send(new RefreshTheTokenCommand(token));

            Response.Cookies.Append("refreshToken", res.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict
            });

            _logger.LogInformation($"Refreshed token with id: {token}");

            return Ok(res.AccessToken);
        }
    }
}
