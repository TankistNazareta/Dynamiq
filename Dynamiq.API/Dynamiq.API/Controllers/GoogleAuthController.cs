using Dynamiq.Application.Commands.GoogleAuth.Commands;
using Dynamiq.Application.Queries.AuthGoogle.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dynamiq.API.Controllers
{
    [ApiController]
    [Route("auth/google")]
    [AllowAnonymous]
    public class GoogleAuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GoogleAuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("log-in")]
        public async Task<IActionResult> Login([FromQuery] string? returnUrl = null)
        {
            var url = await _mediator.Send(new GetLoginGoogleUrlQuery(returnUrl));

            return Redirect(url);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string? code, [FromQuery] string? state, [FromQuery] string? error, CancellationToken ct)
        {
            if (!string.IsNullOrEmpty(error))
                return BadRequest(new { error });
            if (string.IsNullOrEmpty(code))
                return BadRequest(new { error = "Missing authorization code" });

            var authResponse = await _mediator.Send(new GoogleCallbackCommand(code, state));


            Response.Cookies.Append("refreshToken", authResponse.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict
            });
            var frontendUrl = "http://dynamiq-nazareta.fun/auth/callback";
            return Redirect($"{frontendUrl}?accessToken={authResponse.AccessToken}");
        }
    }
}
