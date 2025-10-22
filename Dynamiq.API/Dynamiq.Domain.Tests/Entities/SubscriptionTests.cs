using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Enums;
using FluentAssertions;

namespace Dynamiq.Domain.Tests.Entities
{
    public class SubscriptionTests
    {
        private readonly Guid _userId = Guid.NewGuid();
        private readonly Guid _productId = Guid.NewGuid();
        private readonly Guid _paymentHistoryId = Guid.NewGuid();

        [Fact]
        public void Constructor_ShouldSetDates_For_MonthlyInterval()
        {
            var before = DateTime.UtcNow;
            var sub = new SubscriptionHistory(_userId, _productId, _paymentHistoryId, IntervalEnum.Monthly);
            var after = DateTime.UtcNow;

            sub.UserId.Should().Be(_userId);
            sub.ProductId.Should().Be(_productId);
            sub.PaymentHistoryId.Should().Be(_paymentHistoryId);
            sub.StartDate.Should().BeAfter(before.AddSeconds(-1)).And.BeBefore(after.AddSeconds(1));
            sub.EndDate.Should().BeCloseTo(sub.StartDate.AddMonths(1), precision: TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Constructor_ShouldSetDates_For_YearlyInterval()
        {
            var before = DateTime.UtcNow;
            var sub = new SubscriptionHistory(_userId, _productId, _paymentHistoryId, IntervalEnum.Yearly);
            var after = DateTime.UtcNow;

            sub.StartDate.Should().BeAfter(before.AddSeconds(-1)).And.BeBefore(after.AddSeconds(1));
            sub.EndDate.Should().BeCloseTo(sub.StartDate.AddYears(1), precision: TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Constructor_ShouldThrow_For_UnsupportedInterval()
        {
            Action act = () => new SubscriptionHistory(_userId, _productId, _paymentHistoryId, (IntervalEnum)999);
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void IsActive_ShouldReturnTrue_When_CurrentBetweenStartAndEnd()
        {
            var sub = new SubscriptionHistory(_userId, _productId, _paymentHistoryId, IntervalEnum.Monthly);
            sub.IsActive().Should().BeTrue();
        }

        [Fact]
        public void IsActive_ShouldReturnFalse_When_AfterEndDate()
        {
            var sub = new SubscriptionHistory(_userId, _productId, _paymentHistoryId, IntervalEnum.Monthly);
            typeof(SubscriptionHistory)
                .GetProperty("EndDate")
                .SetValue(sub, DateTime.UtcNow.AddSeconds(-1));
            sub.IsActive().Should().BeFalse();
        }

        [Fact]
        public void Extend_ShouldThrow_When_DurationNotPositive()
        {
            var sub = new SubscriptionHistory(_userId, _productId, _paymentHistoryId, IntervalEnum.Monthly);
            Action act = () => sub.Extend(TimeSpan.FromDays(0));
            act.Should().Throw<ArgumentException>()
                .WithMessage("Duration must be positive");
        }

        [Fact]
        public void Extend_ShouldAddDurationToEndDate_When_Valid()
        {
            var sub = new SubscriptionHistory(_userId, _productId, _paymentHistoryId, IntervalEnum.Monthly);
            var originalEnd = sub.EndDate;
            sub.Extend(TimeSpan.FromDays(5));
            sub.EndDate.Should().BeCloseTo(originalEnd.AddDays(5), precision: TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Cancel_ShouldSetEndDateToNow()
        {
            var sub = new SubscriptionHistory(_userId, _productId, _paymentHistoryId, IntervalEnum.Monthly);
            sub.Cancel();
            sub.EndDate.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(1));
        }
    }
}
