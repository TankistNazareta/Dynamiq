using Dynamiq.Application.Commands.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace Dynamiq.API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public AuthController(IMediator mediator, ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("sign-up")]
        [EnableRateLimiting("SignUpLimiter")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand registeredUser)
        {
            await _mediator.Send(registeredUser);

            _logger.LogInformation("Created user with email: {Email}", registeredUser.Email);

            return Ok(new { Message = "You successfully registered" });
        }

        [HttpPost("log-in")]
        [EnableRateLimiting("LogInLimiter")]
        public async Task<IActionResult> LogIn([FromBody] LogInUserCommand command)
        {
            var res = await _mediator.Send(command);

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

            _logger.LogInformation($"Log in: {res}");

            return Ok();
        }

        [HttpPost("log-out")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("refreshToken");
            return Ok(new { Message = "Logged out" });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new
            {
                Email = User.FindFirst(ClaimTypes.Email)?.Value,
                Role = User.FindFirst(ClaimTypes.Role)?.Value,
                UserId = User.FindFirst("userId")?.Value
            });
        }
    }
}
