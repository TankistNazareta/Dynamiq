using Dynamiq.Domain.Enums;

namespace Dynamiq.Application.DTOs.AccountDTOs
{
    public class SubscriptionDto
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public IntervalEnum Interval { get; private set; }
        public int Price { get; private set; }
    }
}
