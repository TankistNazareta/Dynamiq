using Dynamiq.API.Extension.Enums;
using Dynamiq.API.Extension.RequestEntity;
using Dynamiq.API.Mapping.DTOs;
using Dynamiq.API.Stripe.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;
using Stripe.V2;
using System.Text.Json;

namespace Dynamiq.API.Stripe.Services
{
    public class StripePaymentService : IStripePaymentService
    {
        private readonly StripeClient _client;

        private readonly string _stripeSecretKey;
        private readonly string _webhookSecret;

        private readonly IPaymentHistoryRepo _historyRepo;
        private readonly ISubscriptionRepo _subscriptionRepo;

        public StripePaymentService(IConfiguration config, IPaymentHistoryRepo historyRepo, ISubscriptionRepo subscriptionRepo)
        {
            _stripeSecretKey = config["Stripe:SecretKey"]!;
            _webhookSecret = config["Stripe:WebhookSecret"]!;

            _client = new StripeClient(_stripeSecretKey);

            _historyRepo = historyRepo;
            _subscriptionRepo = subscriptionRepo;
        }

        public async Task<string> CreateCheckoutSession(CheckoutSessionRequest request)
        {
            string ModeType;

            switch(request.PaymentTypeEnum)
            {
                case IntervalEnum.OneTime:
                    ModeType = "payment";
                    break;
                case IntervalEnum.Mountly:
                    ModeType = "subscription";
                    break;
                default:
                    throw new NotImplementedException("Unknown mode type: " + request.PaymentTypeEnum.ToString());
            }

            var paymentHistoryDto = new PaymentHistoryDto()
            {
                Interval = request.PaymentTypeEnum,
                UserId = request.UserId,
                ProductId = request.ProductId,
            };

            var paymentHistoryDtoJson = JsonSerializer.Serialize(paymentHistoryDto);

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = request.StripePriceId,
                        Quantity = request.Quantity
                    }
                },
                Mode = ModeType,
                SuccessUrl = request.SuccessUrl,
                CancelUrl = request.CancelUrl,

                Metadata = new Dictionary<string, string>
                {
                    { "PaymentHistoryDtoJson", paymentHistoryDtoJson }
                }
            };

            var service = new SessionService(_client);
            var session = await service.CreateAsync(options);

            return session.Url;
        }

        public async Task StripeWebhook(string stripeResponseJson, string stripeSignatureHeader)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    stripeResponseJson,
                    stripeSignatureHeader,
                    _webhookSecret
                );

                if (stripeEvent.Type != "checkout.session.completed")
                    return;

                var session = stripeEvent.Data.Object as Session;

                if (session?.Metadata == null || !session.Metadata.TryGetValue("PaymentHistoryDtoJson", out var paymentHistoryDtoJson))
                {
                    throw new Exception("Metadata 'PaymentHistoryDtoJson' not found in session.");
                }

                var paymentHistoryDto = JsonSerializer.Deserialize<PaymentHistoryDto>(paymentHistoryDtoJson);

                if (paymentHistoryDto == null)
                {
                    throw new Exception("Failed to deserialize PaymentHistoryDtoJson.");
                }

                paymentHistoryDto.CreatedAt = DateTime.UtcNow;
                paymentHistoryDto.Amount = session.AmountTotal.HasValue ? session.AmountTotal.Value / 100m : 0m;
                paymentHistoryDto.StripePaymentId = session.PaymentIntentId;

                await _historyRepo.Insert(paymentHistoryDto);

                if(paymentHistoryDto.Interval == IntervalEnum.Mountly 
                    || paymentHistoryDto.Interval == IntervalEnum.Yearly)
                {
                    DateTime endTime;

                    switch(paymentHistoryDto.Interval)
                    {
                        case IntervalEnum.Yearly:
                            endTime = DateTime.UtcNow.AddYears(1);
                            break;
                        case IntervalEnum.Mountly:
                            endTime = DateTime.UtcNow.AddMonths(1);
                            break;
                        default: 
                            throw new Exception("unknown type: " +  paymentHistoryDto.Interval.ToString());
                    }

                    var subscriptionDto = new SubscriptionDto()
                    {
                        ProductId = paymentHistoryDto.ProductId,
                        UserId = paymentHistoryDto.UserId,
                        EndDate = endTime,
                    };

                    await _subscriptionRepo.Insert(subscriptionDto);
                }
            }
            catch (StripeException ex)
            {
                throw new Exception($"Stripe error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Webhook error: {ex.Message}");
            }
        }

    }
}
