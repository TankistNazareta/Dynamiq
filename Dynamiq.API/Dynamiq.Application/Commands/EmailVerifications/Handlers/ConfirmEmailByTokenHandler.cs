using Dynamiq.Application.Commands.EmailVerifications.Commands;
using Dynamiq.Domain.Interfaces.Repositories;
using MediatR;

namespace Dynamiq.Application.Commands.EmailVerifications.Handlers
{
    public class ConfirmEmailByTokenHandler : IRequestHandler<ConfirmEmailByTokenCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepo _repo;

        public ConfirmEmailByTokenHandler(IUnitOfWork unitOfWork, IUserRepo repo)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ConfirmEmailByTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _repo.GetByEmailVerificationTokenAsyc(request.Token, cancellationToken);

            if (user == null)
                throw new KeyNotFoundException("token doesn't exist");

            user.EmailVerification.Confirm(user.Email);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
