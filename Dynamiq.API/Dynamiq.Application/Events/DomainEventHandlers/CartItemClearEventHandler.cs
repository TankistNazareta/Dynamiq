using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Domain.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamiq.Application.Events.DomainEventHandlers
{
    public class CartItemClearEventHandler : INotificationHandler<CartItemClearEvent>
    {
        private readonly ICartItemRepo _cartItemRepo;

        public CartItemClearEventHandler(ICartItemRepo cartItemRepo)
        {
            _cartItemRepo = cartItemRepo;
        }

        public async Task Handle(CartItemClearEvent notification, CancellationToken cancellationToken)
        {
            _cartItemRepo.Remove(notification.CartItem);
        }
    }
}
