using Dynamiq.Domain.Enums;

namespace Dynamiq.Application.DTOs.PaymentDTOs
{
    public record CouponDto
    {
        public Guid Id { get; init; }
        public string Code { get; init; }
        public DiscountTypeEnum DiscountType { get; init; }
        public int DiscountValue { get; init; }
        public DateTime StartTime { get; init; }
        public DateTime EndTime { get; init; }
        public bool IsActiveCoupon { get; init; }
    }
}
