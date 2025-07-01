using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace Dynamiq.API.Controllers
{
    [Route("payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly string _stripeSecretKey;
        private readonly string _webhookSecret;

        private readonly string _successUrl;
        private readonly string _cancelUrl;

        public PaymentController()
        {
            _stripeSecretKey = "sk_test_51ReO9sCMuKRG8STbfedMOzEyGKmDOE4PjRTHmyoHJIPa2ntdYNCTbGWWbLWFIxYyqdNgcuKBNC3gLjb4O1jKkKMc00Cl4909bj";
            _webhookSecret = "whsec_szJm7i7UgaEGGlp1nYkw5hPr9gMB3Wgm";
            _successUrl = "https://deepstatemap.live/";
            _cancelUrl = "https://killer.com/";
        }

        [HttpPost("create-checkout-session")]
        public IActionResult CreateCheckoutSession()
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = 1000,
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Pro підписка"
                        },
                    },
                    Quantity = 1,
                },
            },
                Mode = "payment",
                SuccessUrl = _successUrl,
                CancelUrl = _cancelUrl,
            };


            var client = new StripeClient(_stripeSecretKey);
            var service = new SessionService(client);
            Session session = service.Create(options);

            return Ok(new { url = session.Url });
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            try
            {
                var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                var endpointSecret = _webhookSecret;

                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    endpointSecret
                );

                if (stripeEvent.Type != "checkout.session.completed")
                    throw new Exception($"Payment wasn't successfully: {stripeEvent.Data}");

                var session = stripeEvent.Data.Object as Session;

                return Ok($"Payment successfully: {session.Id}");
            }
            catch (Exception ex)
            {
                return BadRequest("Stripe webhook error: " + ex.Message);
            }
        }
    }
}
