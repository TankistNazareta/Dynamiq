using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Domain.Events;
using MediatR;

namespace Dynamiq.Application.Events.DomainEventHandlers
{
    public class CartItemAddedEventHandler : INotificationHandler<CartItemAddedEvent>
    {
        private readonly ICartItemRepo _cartItemRepo;

        public CartItemAddedEventHandler(ICartItemRepo cartItemRepo)
        {
            _cartItemRepo = cartItemRepo;
        }

        public async Task Handle(CartItemAddedEvent notification, CancellationToken cancellationToken)
        {
            if (notification.CartItem.Id == null || notification.CartItem.Id == Guid.Empty)
            {
                await _cartItemRepo.AddAsync(notification.CartItem, cancellationToken);
            }
            else
            {
                _cartItemRepo.Update(notification.CartItem);
            }
        }
    }
}
