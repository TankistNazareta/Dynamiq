using Dynamiq.Application.Commands.RefreshTokens.Commands;
using Dynamiq.Domain.Interfaces.Repositories;
using MediatR;

namespace Dynamiq.Application.Commands.RefreshTokens.Handlers
{
    public class RevokeRefreshTokenHandler : IRequestHandler<RevokeRefreshTokenCommand>
    {
        private readonly IRefreshTokenRepo _repo;
        private readonly IUnitOfWork _unitOfWork;

        public RevokeRefreshTokenHandler(IRefreshTokenRepo repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var rt = await _repo.GetByTokenAsync(request.Token, cancellationToken);

            if (rt == null)
                throw new KeyNotFoundException($"Refresh Token with token: {request.Token} wasn't found");

            rt.Revoke();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
