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
    public class PaymentHistoryCreatedEventHandler : INotificationHandler<PaymentHistoryCreatedEvent>
    {
        private readonly IProductPaymentHistoryRepo _repo;

        public PaymentHistoryCreatedEventHandler(IProductPaymentHistoryRepo repo)
        {
            _repo = repo;
        }

        public async Task Handle(PaymentHistoryCreatedEvent notification, CancellationToken cancellationToken)
        {
            foreach (var product in notification.PaymentHistory.Products)
            {
                await _repo.AddAsync(product, cancellationToken);
            }
        }
    }
}
