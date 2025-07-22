using Dynamiq.Application.Commands.RefreshTokens.Commands;
using Dynamiq.Application.DTOs;
using Dynamiq.Application.Interfaces.Auth;
using Dynamiq.Domain.Interfaces.Repositories;
using MediatR;

namespace Dynamiq.Application.Commands.RefreshTokens.Handlers
{
    public class RefreshTheTokenHandler : IRequestHandler<RefreshTheTokenCommand, AuthResponseDto>
    {
        private readonly IRefreshTokenRepo _refreshRepo;
        private readonly IUserRepo _userRepo;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public RefreshTheTokenHandler(
            IRefreshTokenRepo refreshRepo, 
            IUserRepo userRepo, 
            ITokenService tokenService,
            IUnitOfWork unitOfWork)
        {
            _refreshRepo = refreshRepo;
            _tokenService = tokenService;
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthResponseDto> Handle(RefreshTheTokenCommand request, CancellationToken cancellationToken)
        {
            var token = await _refreshRepo.GetByTokenAsync(request.RefreshToken, cancellationToken);

            if (token == null)
                throw new KeyNotFoundException("Refresh token wasn`t found");

            var user = await _userRepo.GetByIdAsync(token.UserId, cancellationToken);
            if (token == null)
                throw new KeyNotFoundException("User wasn`t found by his token");

            var authResponseDto = _tokenService.CreateAuthResponse(user.Email, user.Role);

            token.Update(authResponseDto.RefreshToken, false, false);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return authResponseDto;
        }
    }
}
