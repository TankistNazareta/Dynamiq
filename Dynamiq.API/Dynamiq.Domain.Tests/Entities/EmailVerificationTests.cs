using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Events;
using FluentAssertions;
using System.Reflection;

namespace Dynamiq.Domain.Tests.Entities
{
    public class EmailVerificationTests
    {
        private readonly Guid _userId = Guid.NewGuid();
        private const string TestEmail = "user@example.com";

        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            var before = DateTime.UtcNow;
            var ev = new EmailVerification(_userId);
            var after = DateTime.UtcNow;

            ev.UserId.Should().Be(_userId);
            ev.Token.Should().NotBeNullOrWhiteSpace();
            ev.IsConfirmed.Should().BeFalse();
            ev.ExpiresAt.Should().BeAfter(before.AddHours(1)).And.BeBefore(after.AddHours(3));
            ev.DomainEvents.Should().BeEmpty();
        }

        [Fact]
        public void Confirm_ShouldSetIsConfirmedAndRaiseEvent_When_NotExpiredAndNotConfirmed()
        {
            var ev = new EmailVerification(_userId);

            ev.Confirm(TestEmail);

            ev.IsConfirmed.Should().BeTrue();
            ev.DomainEvents.Should().ContainSingle(e => e is UserConfirmedEmailEvent)
                .Which.As<UserConfirmedEmailEvent>().Email.Should().Be(TestEmail);
        }

        [Fact]
        public void Confirm_ShouldThrow_When_AlreadyConfirmed()
        {
            var ev = new EmailVerification(_userId);
            ev.Confirm(TestEmail);

            Action act = () => ev.Confirm(TestEmail);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Email already confirmed.");
        }

        [Fact]
        public void Confirm_ShouldThrow_When_Expired()
        {
            var ev = new EmailVerification(_userId);
            var field = typeof(EmailVerification).GetProperty("ExpiresAt", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            field.SetValue(ev, DateTime.UtcNow.AddHours(-1));

            Action act = () => ev.Confirm(TestEmail);

            act.Should().Throw<TimeoutException>()
                .WithMessage("Verification token expired.");
        }
    }
}
