using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Common;

namespace Dynamiq.Domain.Events
{
    public class UserRegisteredEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; }
        public User User { get; }

        public UserRegisteredEvent(User user)
        {
            User = user;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
