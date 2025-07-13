using Dynamiq.API.Extension.Enums;
using Dynamiq.API.Mapping.DTOs;
using Dynamiq.API.Stripe.Commands.PaymentStripe;
using Dynamiq.API.Stripe.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using System.Text.Json;
using StripeCheckout = Stripe.Checkout;

namespace Dynamiq.API.Stripe.Handlers.PaymentStripe
{
    public class ProcessStripeWebhookHandler : IRequestHandler<ProcessStripeWebhookCommand, PaymentHistoryDto>
    {
        private readonly StripeClient _client;
        private readonly string _webhookSecret;
        private readonly IPaymentHistoryRepo _historyRepo;
        private readonly ISubscriptionRepo _subscriptionRepo;
        private readonly ILogger<ProcessStripeWebhookHandler> _logger;

        public ProcessStripeWebhookHandler(IConfiguration config,
            IPaymentHistoryRepo historyRepo,
            ISubscriptionRepo subscriptionRepo,
            ILogger<ProcessStripeWebhookHandler> logger)
        {
            _client = new StripeClient(config["Stripe:SecretKey"]);
            _webhookSecret = config["Stripe:WebhookSecret"];
            _historyRepo = historyRepo;
            _subscriptionRepo = subscriptionRepo;
            _logger = logger;
        }

        public async Task<PaymentHistoryDto> Handle(ProcessStripeWebhookCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    request.Json,
                    request.Signature,
                    _webhookSecret
                );

                if (stripeEvent.Type != "checkout.session.completed")
                    return null;

                var session = stripeEvent.Data.Object as StripeCheckout.Session;

                if (session?.Metadata == null ||
                    !session.Metadata.TryGetValue("PaymentHistoryDtoJson", out var paymentHistoryDtoJson))
                    throw new KeyNotFoundException("Metadata 'PaymentHistoryDtoJson' not found in session.");

                var paymentHistoryDto = JsonSerializer.Deserialize<PaymentHistoryDto>(paymentHistoryDtoJson);

                if (paymentHistoryDto == null)
                    throw new InvalidDataException("Failed to deserialize PaymentHistoryDtoJson.");

                paymentHistoryDto.CreatedAt = DateTime.UtcNow;
                paymentHistoryDto.Amount = session.AmountTotal.HasValue ? session.AmountTotal.Value / 100m : 0m;
                paymentHistoryDto.StripePaymentId = session.PaymentIntentId;

                await _historyRepo.Insert(paymentHistoryDto);

                if (paymentHistoryDto.Interval == IntervalEnum.Mountly
                    || paymentHistoryDto.Interval == IntervalEnum.Yearly)
                {
                    DateTime endTime;

                    switch (paymentHistoryDto.Interval)
                    {
                        case IntervalEnum.Yearly:
                            endTime = DateTime.UtcNow.AddYears(1);
                            break;
                        case IntervalEnum.Mountly:
                            endTime = DateTime.UtcNow.AddMonths(1);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(paymentHistoryDto.Interval),
                                $"Unsupported interval type: {paymentHistoryDto.Interval}");
                    }

                    var subscriptionDto = new SubscriptionDto
                    {
                        ProductId = paymentHistoryDto.ProductId,
                        UserId = paymentHistoryDto.UserId,
                        EndDate = endTime,
                    };

                    await _subscriptionRepo.Insert(subscriptionDto);
                }

                return paymentHistoryDto;
            }
            catch (StripeException ex)
            {
                throw new ApplicationException($"Stripe error: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException("Invalid JSON format for PaymentHistoryDtoJson.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unexpected error in Stripe webhook.", ex);
            }
        }
    }
}
