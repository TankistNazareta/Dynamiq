using Dynamiq.Application.Common;
using Dynamiq.Application.Interfaces.Services;
using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Events;
using Dynamiq.Domain.Interfaces.Repositories;
using MediatR;

namespace Dynamiq.Application.Events.Handlers
{
    public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredEvent>
    {
        private readonly IEmailVerificationRepo _emailVerificationRepo;
        private readonly IEmailService _emailService;

        public UserRegisteredEventHandler(
            IEmailVerificationRepo emailVerificationRepo, IEmailService emailService)
        {
            _emailVerificationRepo = emailVerificationRepo;
            _emailService = emailService;
        }

        public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
        {
            var verificationToken = new EmailVerification(notification.UserId);

            await _emailVerificationRepo.AddAsync(verificationToken, cancellationToken);

            await _emailService.SendEmailAsync(notification.Email, "Confirm Your Email", HtmlBodyForEmail.GetSignInBody(verificationToken.Token));
        }
    }
}
