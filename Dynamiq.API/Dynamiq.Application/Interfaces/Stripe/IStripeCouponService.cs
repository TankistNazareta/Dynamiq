namespace Dynamiq.Application.Interfaces.Stripe
{
    public interface IStripeCouponService
    {
        Task<string> CreateStripeCouponAsync(double discountAmount);
        Task DeactivateCoupon(string stripeCouponId);
    }
}
