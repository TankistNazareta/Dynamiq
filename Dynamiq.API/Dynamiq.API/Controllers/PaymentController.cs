using Dynamiq.API.Extension.RequestEntity;
using Dynamiq.API.Stripe.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace Dynamiq.API.Controllers
{
    [Route("payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IStripePaymentService _service;

        public PaymentController(IStripePaymentService service)
        {
            _service = service;
        }

        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutSessionRequest request)
        {
            try
            {
                var sessionUrl = await _service.CreateCheckoutSession(request);
                return Ok(new { url = sessionUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            string json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            string stripeSignature = Request.Headers["Stripe-Signature"];

            try
            {
                await _service.StripeWebhook(json, stripeSignature);
                return Ok(); // 200
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
