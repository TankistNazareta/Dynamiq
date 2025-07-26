using Dynamiq.Domain.Common;

namespace Dynamiq.Domain.Events
{
    public class UserChangedPasswordEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; }
        public string Email { get; }

        public UserChangedPasswordEvent(string email)
        {
            Email = email;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
