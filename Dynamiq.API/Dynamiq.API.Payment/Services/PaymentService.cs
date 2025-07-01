//using Dynamiq.API.Payment.Interfaces;
//using Stripe;
//using Microsoft.Extensions.Configuration;
//using Stripe.V2;

//namespace Dynamiq.API.Payment.Services
//{
//    public class PaymentService : IPaymentService
//    {
//        private readonly string _apiKey;
//        //"whsec_gaily-warm-glory-safely"
//        private readonly string _webhookSecret;

//        public PaymentService(IConfiguration configuration)
//        {
//            _apiKey = configuration["Stripe:SecretKey"];
//            _webhookSecret = configuration["Stripe:WebhookSecret"];
//            StripeConfiguration.ApiKey = _apiKey;
//        }

//        public async Task<string> CreatePaymentIntent(long amount, string currency)
//        {
//            var options = new PaymentIntentCreateOptions
//            {
//                Amount = amount,
//                Currency = currency,
//                PaymentMethodTypes = new List<string> { "card" },
//            };

//            var service = new PaymentIntentService();
//            var paymentIntent = await service.CreateAsync(options);

//            return paymentIntent.ClientSecret;
//        }

//        public async Task<string> CreateSubscriptionAsync(string priceId, string customerEmail)
//        {
//            var customerService = new CustomerService();
//            var customer = await customerService.CreateAsync(new CustomerCreateOptions
//            {
//                Email = customerEmail
//            });

//            var subscriptionOptions = new SubscriptionCreateOptions
//            {
//                Customer = customer.Id,
//                Items = new List<SubscriptionItemOptions>
//        {
//            new SubscriptionItemOptions
//            {
//                Price = priceId
//            }
//        },
//                PaymentBehavior = "default_incomplete",
//                Expand = new List<string> { "latest_invoice.payment_intent" }
//            };

//            var subscriptionService = new SubscriptionService();
//            var subscription = await subscriptionService.CreateAsync(subscriptionOptions);

//            // Ось тут latest_invoice вже буде розширено
//            var invoice = subscription.LatestInvoice as Invoice;
//            var paymentIntent = invoice.PaymentIntent as PaymentIntent;

//            return paymentIntent.ClientSecret;
//        }

//    }
//}
