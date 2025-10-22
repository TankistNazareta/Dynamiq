﻿using Dynamiq.Application.DTOs.PaymentDTOs;
using Dynamiq.Application.Interfaces.Stripe;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Stripe;
using Stripe.Checkout;
using System.Text.Json;

namespace Dynamiq.Infrastructure.Services.Stripes
{
    public class StripeWebhookParser : IStripeWebhookParser
    {
        private readonly string _webhookSecret;

        public StripeWebhookParser(IConfiguration config)
        {
            _webhookSecret = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET")!
                ?? throw new ArgumentNullException("STRIPE_WEBHOOK_SECRET is not configured");
        }

        public string GetEventType(string json, string signature)
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                signature,
                _webhookSecret
            );

            return stripeEvent.Type;
        }

        public WebhookParserDto ParseCheckoutSessionCompleted(string json, string signature)
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                signature,
                _webhookSecret
            );

            var session = stripeEvent.Data.Object as Session;

            if (session?.Metadata == null ||
                !session.Metadata.TryGetValue("WebhookParserDto", out var paymentHistoryDtoJson))
                throw new KeyNotFoundException("Metadata 'WebhookParserDto' not found in session.");

            var paymentHistoryDto = JsonSerializer.Deserialize<WebhookParserDto>(paymentHistoryDtoJson);

            if (paymentHistoryDto == null)
                throw new InvalidDataException("Failed to deserialize WebhookParserDtoJson.");

            paymentHistoryDto.Amount = session.AmountTotal.HasValue ? session.AmountTotal.Value / 100m : 0m;

            var stripeTransactionId = session.PaymentIntentId ?? session.SubscriptionId;
            paymentHistoryDto.StripeTransactionId = stripeTransactionId;

            return paymentHistoryDto;
        }

        public string ParseDeletedSubscriptionId(string json, string signature)
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                signature,
                _webhookSecret
            );

            var subscription = stripeEvent.Data.Object as Subscription;
            return subscription.Id;
        }

        public CouponsResultDto? TryGetCoupons(string json, string signature)
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                signature,
                _webhookSecret
            );

            var session = stripeEvent.Data.Object as Session;

            string coupons = null;
            string stripeCouponIds = null;

            session?.Metadata?.TryGetValue("Coupons", out coupons);
            session?.Metadata?.TryGetValue("CouponStripeIds", out stripeCouponIds);

            if (stripeCouponIds == null || coupons == null)
                return null;

            var couponsCodeList = JsonSerializer.Deserialize<List<string>>(coupons);
            var stripeCouponIdList = JsonSerializer.Deserialize<List<string>>(stripeCouponIds);

            return new(couponsCodeList, stripeCouponIdList);
        }
    }
}