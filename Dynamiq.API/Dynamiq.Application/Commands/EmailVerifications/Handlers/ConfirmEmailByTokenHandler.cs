using Dynamiq.Application.Commands.EmailVerifications.Commands;
using Dynamiq.Domain.Interfaces.Repositories;
using MediatR;

namespace Dynamiq.Application.Commands.EmailVerifications.Handlers
{
    public class ConfirmEmailByTokenHandler : IRequestHandler<ConfirmEmailByTokenCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailVerificationRepo _repo;

        public ConfirmEmailByTokenHandler(IUnitOfWork unitOfWork, IEmailVerificationRepo repo)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ConfirmEmailByTokenCommand request, CancellationToken cancellationToken)
        {
            var ev = await _repo.GetByTokenAsync(request.Token, cancellationToken);

            if (ev == null)
                throw new KeyNotFoundException("token doesn't exist");

            ev.Confirm();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
