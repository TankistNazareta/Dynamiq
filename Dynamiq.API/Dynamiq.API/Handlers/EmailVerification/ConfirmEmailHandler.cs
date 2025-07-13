using Dynamiq.API.Commands.EmailVerification;
using Dynamiq.API.DAL.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dynamiq.API.Handlers.EmailVerification
{
    public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand>
    {
        private readonly AppDbContext _db;
        private readonly ILogger<ConfirmEmailHandler> _logger;

        public ConfirmEmailHandler(AppDbContext db, ILogger<ConfirmEmailHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var model = await _db.EmailVerifications.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (model is null)
                throw new KeyNotFoundException($"Email verification with id: {request.Id} does not exist or is expired");

            if (model.ExpiresAt < DateTime.UtcNow)
                throw new TimeoutException("Token expired");

            if (model.ConfirmedEmail)
                throw new InvalidOperationException("This token has already been activated.");

            model.ConfirmedEmail = true;

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Email Verification with id: {Id} was confirmed", request.Id);
        }
    }
}
