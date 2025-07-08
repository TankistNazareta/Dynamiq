using Dynamiq.API.Extension.Interfaces;
using Dynamiq.API.Mapping.DTOs;

namespace Dynamiq.API.Stripe.Interfaces
{
    public interface ISubscriptionRepo : ICRUD<SubscriptionDto>
    {
        Task<bool> HaveActiveSubscription(Guid userId);
    }
}
