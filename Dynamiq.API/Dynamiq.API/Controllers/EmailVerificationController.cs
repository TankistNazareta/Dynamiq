using Dynamiq.Application.Commands.EmailVerifications.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dynamiq.API.Controllers
{
    [Route("email-verification")]
    [ApiController]
    public class EmailVerificationController : ControllerBase
    {
        private readonly ILogger<EmailVerificationController> _logger;
        private readonly IMediator _mediator;

        public EmailVerificationController(ILogger<EmailVerificationController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPut("confirm")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
        {
            await _mediator.Send(new ConfirmEmailByTokenCommand(token));

            _logger.LogInformation("Email Verification with token: {Token} was confirmed", token);

            return Ok("You confirmed email successfully");
        }
    }
}
