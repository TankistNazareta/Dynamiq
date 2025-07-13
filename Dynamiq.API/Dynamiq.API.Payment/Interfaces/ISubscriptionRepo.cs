using Dynamiq.API.Extension.Interfaces;
using Dynamiq.API.Mapping.DTOs;

namespace Dynamiq.API.Stripe.Interfaces
{
    public interface ISubscriptionRepo : ICrudRepo<SubscriptionDto>
    {
        Task<bool> HasActiveSubscription(Guid userId);
    }
}
