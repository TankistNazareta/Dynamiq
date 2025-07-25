using Dynamiq.Application.Common;
using Dynamiq.Application.Interfaces.Services;
using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Events;
using MediatR;

namespace Dynamiq.Application.Events.DomainEventHandlers
{
    public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredEvent>
    {
        private readonly IEmailService _emailService;

        public UserRegisteredEventHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
        {
            var user = notification.User;

            var verificationToken = new EmailVerification(user.Id);

            user.SetEmailVerification(verificationToken);

            await _emailService.SendEmailAsync(user.Email, "Confirm Your Email", HtmlBodyForEmail.GetSignInBody(verificationToken.Token));
        }
    }
}
