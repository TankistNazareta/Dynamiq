using Dynamiq.Application.Commands.RefreshTokens.Commands;
using Dynamiq.Domain.Interfaces.Repositories;
using MediatR;

namespace Dynamiq.Application.Commands.RefreshTokens.Handlers
{
    public class RevokeRefreshTokenHandler : IRequestHandler<RevokeRefreshTokenCommand>
    {
        private readonly IUserRepo _repo;
        private readonly IUnitOfWork _unitOfWork;

        public RevokeRefreshTokenHandler(IUserRepo repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _repo.GetByRefreshTokenAsync(request.Token, cancellationToken);

            if (user == null)
                throw new KeyNotFoundException($"Refresh Token with token: {request.Token} wasn't found");

            user.RevokeRefreshToken(request.Token);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
