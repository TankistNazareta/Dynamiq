using Dynamiq.Application.Common;
using Dynamiq.Application.Interfaces.Services;
using Dynamiq.Domain.Events;
using MediatR;

namespace Dynamiq.Application.Events.DomainEventHandlers
{
    public class UserConfirmedEmailEventHandler : INotificationHandler<UserConfirmedEmailEvent>
    {
        private readonly IEmailService _emailService;

        public UserConfirmedEmailEventHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task Handle(UserConfirmedEmailEvent notification, CancellationToken cancellationToken)
        {
            await _emailService.SendEmailAsync(notification.Email, "Welcome to Dynamiq", HtmlBodyForEmail.GetWelcomeBody(notification.Email));
        }
    }
}
