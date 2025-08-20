using Dynamiq.Application.Commands.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Dynamiq.API.Controllers
{
    [ApiController]
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

            return Ok("You successfully registered");
        }

        [HttpPost("log-in")]
        [EnableRateLimiting("LogInLimiter")]
        public async Task<IActionResult> LogIn([FromBody] LogInUserCommand command)
        {
            var res = await _mediator.Send(command);

            _logger.LogInformation($"Log in: {res}");

            return Ok(new { AccessToken = res.AccessToken });
        }

        [HttpPost("log-out")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("refreshToken");
            return Ok("Logged out");
        }

    }
}
