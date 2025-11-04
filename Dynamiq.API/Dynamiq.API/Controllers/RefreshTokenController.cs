using Dynamiq.Application.Commands.RefreshTokens.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpPut("revoke")]
        public async Task<IActionResult> Revoke()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var token))
                return Unauthorized(new { message = "No refresh token" });

            await _mediator.Send(new RevokeRefreshTokenCommand(token));

            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");

            _logger.LogInformation("Refresh token revoked: {Token}", token);

            return Ok(new { Message = "Token successfully revoked" });
        }

        [AllowAnonymous]
        [HttpPut("refresh")]
        public async Task<IActionResult> Refresh()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var token))
                return Unauthorized(new { message = "No refresh token" });

            var res = await _mediator.Send(new RefreshTheTokenCommand(token));

            Response.Cookies.Append("accessToken", res.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddHours(1)
            });

            Response.Cookies.Append("refreshToken", res.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });


            _logger.LogInformation($"Refreshed token with id: {token}");

            return Ok();
        }
    }
}
