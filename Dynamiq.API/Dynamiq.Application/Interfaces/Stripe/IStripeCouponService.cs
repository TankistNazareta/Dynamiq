namespace Dynamiq.Application.Interfaces.Stripe
{
    public interface IStripeCouponService
    {
        Task<string> CreateStripeCouponAsync(int discountAmount);
        Task DeactivateCoupon(string stripeCouponId);
    }
}
