namespace Dynamiq.Domain.Entities
{
    public class EmailVerification
    {
        public Guid Id { get; private set; }
        public string Token { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool IsConfirmed { get; private set; }
        public Guid UserId { get; private set; }

        private EmailVerification() { } // EF Core

        public EmailVerification(Guid userId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Token = Guid.NewGuid().ToString();
            ExpiresAt = DateTime.UtcNow.AddHours(2);
            IsConfirmed = false;
        }

        public void Confirm()
        {
            if (IsConfirmed)
                throw new InvalidOperationException("Email already confirmed.");

            if (ExpiresAt < DateTime.UtcNow)
                throw new TimeoutException("Verification token expired.");

            IsConfirmed = true;
        }
    }
}
