using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Domain.Events;
using MediatR;

namespace Dynamiq.Application.Events.DomainEventHandlers
{
    public class CartClearedEventHandler : INotificationHandler<CartClearedEvent>
    {
        private readonly ICartItemRepo _cartItemRepo;

        public CartClearedEventHandler(ICartItemRepo cartItemRepo)
        {
            _cartItemRepo = cartItemRepo;
        }

        public async Task Handle(CartClearedEvent notification, CancellationToken cancellationToken)
        {
            var items = await _cartItemRepo.GetByCartIdAsync(notification.CartId, cancellationToken);

            foreach (var item in items)
            {
                _cartItemRepo.Remove(item);
            }
        }
    }
}
