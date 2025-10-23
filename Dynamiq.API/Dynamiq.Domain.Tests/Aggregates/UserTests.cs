using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Enums;
using Dynamiq.Domain.Events;
using FluentAssertions;

namespace Dynamiq.Domain.Tests.Aggregates
{
    public class UserTests
    {
        private const string ValidEmail = "test@example.com";
        private const string ValidPasswordHash = "hashed";

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_ShouldThrow_When_EmailInvalid(string email)
        {
            Action act = () => new User(email, ValidPasswordHash, RoleEnum.DefaultUser);
            act.Should().Throw<ArgumentException>()
                .WithParameterName("email");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_ShouldThrow_When_PasswordHashInvalid(string pwd)
        {
            Action act = () => new User(ValidEmail, pwd, RoleEnum.DefaultUser);
            act.Should().Throw<ArgumentException>()
                .WithParameterName("passwordHash");
        }

        [Fact]
        public void Constructor_ShouldRaiseUserRegisteredEvent_When_Valid()
        {
            var user = new User(ValidEmail, ValidPasswordHash, RoleEnum.Admin);
            user.Email.Should().Be(ValidEmail);
            user.PasswordHash.Should().Be(ValidPasswordHash);
            user.Role.Should().Be(RoleEnum.Admin);
            user.PaymentHistories.Should().BeEmpty();
            user.RefreshTokens.Should().BeEmpty();
            user.EmailVerification.Should().BeNull();
            user.DomainEvents.Should().ContainSingle(e => e is UserRegisteredEvent);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ChangePassword_ShouldThrow_When_NewHashInvalid(string newHash)
        {
            var user = new User(ValidEmail, ValidPasswordHash, RoleEnum.DefaultUser);
            Action act = () => user.ChangePassword(newHash);
            act.Should().Throw<ArgumentException>()
                .WithParameterName("newPasswordHash");
        }

        [Fact]
        public void ChangePassword_ShouldUpdateHashAndRaiseEvent_When_Valid()
        {
            var user = new User(ValidEmail, ValidPasswordHash, RoleEnum.DefaultUser);
            user.ClearDomainEvents();

            user.ChangePassword("newHash");

            user.PasswordHash.Should().Be("newHash");
            user.DomainEvents.Should().ContainSingle(e => e is UserChangedPasswordEvent);
        }

        [Fact]
        public void AddRefreshToken_ShouldAddToken()
        {
            var user = new User(ValidEmail, ValidPasswordHash, RoleEnum.DefaultUser);
            var rt = new RefreshToken(user.Id, "token");

            user.AddRefreshToken(rt);

            user.RefreshTokens.Should().ContainSingle().Which.Should().Be(rt);
        }

        [Fact]
        public void UpdateToken_ShouldThrow_When_TokenNotFound()
        {
            var user = new User(ValidEmail, ValidPasswordHash, RoleEnum.DefaultUser);
            Action act = () => user.UpdateToken("old", "new");
            act.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public void RevokeRefreshToken_ShouldThrow_When_TokenNotFound()
        {
            var user = new User(ValidEmail, ValidPasswordHash, RoleEnum.DefaultUser);
            Action act = () => user.RevokeRefreshToken("invalid");
            act.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public void SetEmailVerification_ShouldAssignEntity()
        {
            var user = new User(ValidEmail, ValidPasswordHash, RoleEnum.DefaultUser);
            var ev = new EmailVerification(user.Id);

            user.SetEmailVerification(ev);

            user.EmailVerification.Should().Be(ev);
        }
    }
}
