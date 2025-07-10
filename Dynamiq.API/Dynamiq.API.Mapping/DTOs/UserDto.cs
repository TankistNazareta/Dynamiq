using Dynamiq.API.Extension.Enums;

namespace Dynamiq.API.Mapping.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public RoleEnum Role { get; set; }
        public RefreshTokenDto RefreshToken { get; set; } = null!;
        public ICollection<PaymentHistoryDto> PaymentHistories { get; set; }
        public ICollection<SubscriptionDto> Subscriptions { get; set; }
        public EmailVerificationDto EmailVerification { get; set; }
    }
}
