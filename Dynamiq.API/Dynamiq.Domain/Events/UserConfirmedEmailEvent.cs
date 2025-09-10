using Dynamiq.Domain.Common;

namespace Dynamiq.Domain.Events
{
    public class UserConfirmedEmailEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; }
        public string Email { get; }
        public Guid UserId { get; private set; }

        public UserConfirmedEmailEvent(string email, Guid userId)
        {
            Email = email;
            OccurredOn = DateTime.UtcNow;
            UserId = userId;
        }
    }
}
