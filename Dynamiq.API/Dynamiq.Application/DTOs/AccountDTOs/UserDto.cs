using Dynamiq.Domain.Enums;

namespace Dynamiq.Application.DTOs.AccountDTOs
{
    public class UserDto
    {
        public Guid Id { get; init; }
        public string Email { get; init; }
        public RoleEnum Role { get; init; }
        public EmailVerificationDto EmailVerification { get; init; }
        public PaymentHistoryDto[] PaymentHistories { get; init; }
        public SubscriptionDto Subscription { get; init; }
    }
}
