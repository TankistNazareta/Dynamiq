using Dynamiq.Domain.Common;
using Dynamiq.Domain.Enums;
using Dynamiq.Domain.Events;

namespace Dynamiq.Domain.Entities
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

        // EF Core needs parameterless constructor
        private User() { }

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

            AddDomainEvent(new UserRegisteredEvent(Id, Email));
        }

        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("Password cannot be empty.", nameof(newPasswordHash));

            PasswordHash = newPasswordHash;
        }

        public void AddSubscription(Subscription subscription)
        {
            _subscriptions.Add(subscription);
        }

        public void AddPaymentHistory(PaymentHistory payment)
        {
            _paymentHistories.Add(payment);
        }

        public void SetRefreshToken(RefreshToken refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }
}