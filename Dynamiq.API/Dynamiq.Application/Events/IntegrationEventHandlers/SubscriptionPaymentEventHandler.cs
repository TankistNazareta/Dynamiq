using Dynamiq.Application.IntegrationEvents;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Domain.Entities;
using MediatR;

namespace Dynamiq.Application.Events.IntegrationEventHandlers
{
    public class SubscriptionPaymentEventHandler : INotificationHandler<SubscriptionPaymentEvent>
    {
        private readonly IUserRepo _userRepo;

        public SubscriptionPaymentEventHandler(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task Handle(SubscriptionPaymentEvent notification, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetByIdAsync(notification.UserId, cancellationToken);

            if (user == null)
                throw new KeyNotFoundException($"user with id: {notification.UserId} doesn't exist");

            var sub = new Subscription(user.Id, notification.ProductId, notification.PaymentHistoryId, notification.Interval);

            user.AddSubscription(sub);
        }
    }
}
