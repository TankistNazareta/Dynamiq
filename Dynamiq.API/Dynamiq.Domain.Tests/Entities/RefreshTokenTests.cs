using Dynamiq.Domain.Entities;
using FluentAssertions;
using System.Reflection;

namespace Dynamiq.Domain.Tests.Entities
{
    public class RefreshTokenTests
    {
        private readonly Guid _userId = Guid.NewGuid();
        private const string InitialToken = "initial";

        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            var before = DateTime.UtcNow;
            var rt = new RefreshToken(_userId, InitialToken);
            var after = DateTime.UtcNow;

            rt.UserId.Should().Be(_userId);
            rt.Token.Should().Be(InitialToken);
            rt.IsRevoked.Should().BeFalse();
            rt.ExpiresAt.Should().BeAfter(before.AddDays(6.9)).And.BeBefore(after.AddDays(7.1));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void UpdateToken_ShouldThrow_When_TokenInvalid(string token)
        {
            var rt = new RefreshToken(_userId, InitialToken);
            Action act = () => rt.UpdateToken(token);
            act.Should().Throw<ArgumentException>()
                .WithParameterName("token");
        }

        [Fact]
        public void UpdateToken_ShouldResetExpiresAndRevoked_When_Valid()
        {
            var rt = new RefreshToken(_userId, InitialToken);
            rt.Revoke();
            var before = DateTime.UtcNow;

            rt.UpdateToken("newtoken");

            rt.Token.Should().Be("newtoken");
            rt.IsRevoked.Should().BeFalse();
            rt.ExpiresAt.Should().BeAfter(before.AddDays(6.9)).And.BeBefore(DateTime.UtcNow.AddDays(7.1));
        }

        [Fact]
        public void Revoke_ShouldSetIsRevokedTrue()
        {
            var rt = new RefreshToken(_userId, InitialToken);
            rt.Revoke();
            rt.IsRevoked.Should().BeTrue();
        }

        [Fact]
        public void Revoke_ShouldThrow_When_AlreadyRevoked()
        {
            var rt = new RefreshToken(_userId, InitialToken);
            rt.Revoke();
            Action act = () => rt.Revoke();
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Token already revoked.");
        }

        [Fact]
        public void IsActive_ShouldBeTrue_When_NotRevokedAndNotExpired()
        {
            var rt = new RefreshToken(_userId, InitialToken);
            rt.IsActive().Should().BeTrue();
        }

        [Fact]
        public void IsActive_ShouldBeFalse_When_Expired()
        {
            var rt = new RefreshToken(_userId, InitialToken);
            var prop = typeof(RefreshToken).GetProperty("ExpiresAt", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            prop.SetValue(rt, DateTime.UtcNow.AddSeconds(-1));
            rt.IsActive().Should().BeFalse();
        }

        [Fact]
        public void IsActive_ShouldBeFalse_When_Revoked()
        {
            var rt = new RefreshToken(_userId, InitialToken);
            rt.Revoke();
            rt.IsActive().Should().BeFalse();
        }

        [Fact]
        public void DefaultTimeForExpireAt_ShouldReturnUtcNowPlusSevenDays()
        {
            var rt = new RefreshToken(_userId, InitialToken);
            var expected = DateTime.UtcNow.AddDays(7);
            rt.DefaultTimeForExpireAt.Should().BeCloseTo(expected, precision: TimeSpan.FromSeconds(1));
        }
    }
}
