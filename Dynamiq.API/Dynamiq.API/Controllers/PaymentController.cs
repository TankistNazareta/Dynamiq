using Dynamiq.API.Extension.RequestEntity;
using Dynamiq.API.Stripe.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Dynamiq.API.Controllers
{
    [Route("payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IStripePaymentService _service;
        private readonly ILogger _logger;

        public PaymentController(IStripePaymentService service, ILogger<PaymentController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("create-checkout-session")]
        [EnableRateLimiting("CreateCheckoutLimiter")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutSessionRequest request)
        {
            var sessionUrl = await _service.CreateCheckoutSession(request);

            _logger.LogInformation("checkout session was created for user: {Id}", request.UserId);

            return Ok(new { url = sessionUrl });
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            string json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            string stripeSignature = Request.Headers["Stripe-Signature"];

            var res = await _service.StripeWebhook(json, stripeSignature);

            _logger.LogInformation("payment completed successfully, payment history id: {Id}", res.Id);

            return Ok("completed successfully");
        }
    }
}
