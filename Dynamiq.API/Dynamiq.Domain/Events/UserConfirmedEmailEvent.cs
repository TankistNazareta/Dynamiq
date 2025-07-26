using Dynamiq.Domain.Common;

namespace Dynamiq.Domain.Events
{
    public class UserConfirmedEmailEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; }
        public string Email { get; }

        public UserConfirmedEmailEvent(string email)
        {
            Email = email;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
