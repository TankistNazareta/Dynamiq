namespace Dynamiq.Domain.Entities
{
    public class RefreshToken
    {
        public DateTime DefaultTimeForExpireAt => DateTime.UtcNow.AddDays(7);

        public Guid Id { get; private set; }
        public string Token { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool IsRevoked { get; private set; }
        public Guid UserId { get; private set; }

        private RefreshToken() { } // EF Core

        public RefreshToken(Guid userId, string token)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            UpdateToken(token);
            IsRevoked = false;
        }

        public void UpdateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be empty", nameof(token));

            Token = token;
            ExpiresAt = DateTime.UtcNow.AddDays(7);
            IsRevoked = false;
        }

        public void Revoke()
        {
            if (IsRevoked)
                throw new InvalidOperationException("Token already revoked.");

            IsRevoked = true;
        }

        public bool IsActive()
        {
            return !IsRevoked && ExpiresAt > DateTime.UtcNow;
        }
    }
}