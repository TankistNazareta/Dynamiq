using Dynamiq.Domain.Enums;

namespace Dynamiq.Domain.Entities
{
    public class Coupon
    {
        public Guid Id { get; private set; }
        public string Code { get; private set; }
        public DiscountTypeEnum DiscountType { get; private set; }
        public int DiscountValue { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public bool IsActiveCoupon { get; private set; }

        private Coupon() {} // For EF

        public Coupon(
            string code, 
            DiscountTypeEnum discountType,
            int discountValue,
            DateTime startTime,
            DateTime endTime)
        {
            Code = code;
            DiscountType = discountType;
            DiscountValue = discountValue;
            StartTime = startTime;
            EndTime = endTime;
            IsActiveCoupon = true;
        }

        public bool IsActive()
            => IsActiveCoupon && EndTime > DateTime.UtcNow;

        public void DeactivateCoupon()
        {
            IsActiveCoupon = false;
        }
    }
}
