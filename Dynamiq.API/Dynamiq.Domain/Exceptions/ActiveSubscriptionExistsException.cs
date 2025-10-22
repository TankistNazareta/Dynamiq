namespace Dynamiq.Domain.Exceptions
{
    public class ActiveSubscriptionExistsException : Exception
    {
        public ActiveSubscriptionExistsException(string message = "User already has an active subscription.")
            : base(message)
        {
        }
    }
}
