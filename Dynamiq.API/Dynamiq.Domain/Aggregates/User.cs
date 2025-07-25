using Dynamiq.Domain.Common;
using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Enums;
using Dynamiq.Domain.Events;

namespace Dynamiq.Domain.Aggregates
{
    public class User : BaseEntity
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public RoleEnum Role { get; private set; }

        private readonly List<PaymentHistory> _paymentHistories = new();
        public IReadOnlyCollection<PaymentHistory> PaymentHistories => _paymentHistories.AsReadOnly();

        private readonly List<Subscription> _subscriptions = new();
        public IReadOnlyCollection<Subscription> Subscriptions => _subscriptions.AsReadOnly();

        public RefreshToken RefreshToken { get; private set; }
        public EmailVerification EmailVerification { get; private set; }

        private User() { } // EF Core

        public User(string email, string passwordHash, RoleEnum role)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.", nameof(email));

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password cannot be empty.", nameof(passwordHash));

            Id = Guid.NewGuid();
            Email = email;
            PasswordHash = passwordHash;
            Role = role;

            AddDomainEvent(new UserRegisteredEvent(this));
        }

        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("Password cannot be empty.", nameof(newPasswordHash));

            PasswordHash = newPasswordHash;
        }

        public void AddSubscription(Subscription subscription)
        {
            if (_subscriptions.Any(s => s.IsActive()))
                throw new InvalidOperationException("User can't have 2 or more active subscription, please contact with support");

            _subscriptions.Add(subscription);
        }

        public void SetRefreshToken(RefreshToken refreshToken)
                => RefreshToken = refreshToken;

        public void SetEmailVerification(EmailVerification emailVerification)
                => EmailVerification = emailVerification;
    }
}