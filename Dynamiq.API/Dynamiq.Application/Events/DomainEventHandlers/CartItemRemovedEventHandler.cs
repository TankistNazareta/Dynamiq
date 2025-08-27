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
    internal class CartItemRemovedEventHandler : INotificationHandler<CartItemRemovedEvent>
    {
        private readonly ICartItemRepo _cartItemRepo;

        public CartItemRemovedEventHandler(ICartItemRepo cartItemRepo)
        {
            _cartItemRepo = cartItemRepo;
        }

        public async Task Handle(CartItemRemovedEvent notification, CancellationToken cancellationToken)
        {
            _cartItemRepo.Update(notification.CartItem);
        }
    }
}
