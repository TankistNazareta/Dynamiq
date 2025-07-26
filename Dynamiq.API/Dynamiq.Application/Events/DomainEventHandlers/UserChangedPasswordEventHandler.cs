using Dynamiq.Application.Interfaces.Services;
using Dynamiq.Domain.Events;
using MediatR;

namespace Dynamiq.Application.Events.DomainEventHandlers
{
    public class UserChangedPasswordEventHandler : INotificationHandler<UserChangedPasswordEvent>
    {
        private readonly IEmailService _emailService;

        public UserChangedPasswordEventHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task Handle(UserChangedPasswordEvent notification, CancellationToken cancellationToken)
        {
            await _emailService.SendEmailAsync(notification.Email, "Your password was changed",
                    "If this wasn't you, please contact support immediately.");
        }
    }
}
