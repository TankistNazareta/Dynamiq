using Dynamiq.Domain.Common;

namespace Dynamiq.Domain.Events
{
    public class UserRegisteredEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; }
        public Guid UserId { get; }
        public string Email { get; }

        public UserRegisteredEvent(Guid userId, string email)
        {
            UserId = userId;
            Email = email;
            OccurredOn = DateTime.Now;
        }
    }
}
