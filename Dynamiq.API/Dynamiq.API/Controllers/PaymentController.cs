using Dynamiq.Application.Commands.Payment.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Dynamiq.API.Controllers
{
    [Route("payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public PaymentController(ILogger<PaymentController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("create-checkout-session")]
        [EnableRateLimiting("CreateCheckoutLimiter")]
        [Authorize]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionCommand request)
        {
            var sessionUrl = await _mediator.Send(request);

            _logger.LogInformation("checkout session was created for user: {Id}", request.UserId);

            return Ok(new { Url = sessionUrl });
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> StripeWebhook()
        {
            string json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            string stripeSignature = Request.Headers["Stripe-Signature"];

            var needResponse = await _mediator.Send(new StripeWebhookCommand(json, stripeSignature));

            if (!needResponse)
                return Ok();

            _logger.LogInformation("payment completed successfully");

            return Ok(new { Message = "completed successfully" });
        }
    }
}
