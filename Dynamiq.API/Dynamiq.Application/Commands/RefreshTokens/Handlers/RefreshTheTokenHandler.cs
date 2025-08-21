using Dynamiq.Application.Commands.RefreshTokens.Commands;
using Dynamiq.Application.DTOs.AuthDTOs;
using Dynamiq.Application.Interfaces.Auth;
using Dynamiq.Domain.Interfaces.Repositories;
using MediatR;

namespace Dynamiq.Application.Commands.RefreshTokens.Handlers
{
    public class RefreshTheTokenHandler : IRequestHandler<RefreshTheTokenCommand, AuthTokensDto>
    {
        private readonly IUserRepo _userRepo;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public RefreshTheTokenHandler(
            IUserRepo userRepo, 
            ITokenService tokenService,
            IUnitOfWork unitOfWork)
        {
            _tokenService = tokenService;
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthTokensDto> Handle(RefreshTheTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);

            if (user == null)
                throw new KeyNotFoundException("User wasn`t found by his refresh token");

            var authResponseDto = _tokenService.CreateAuthResponse(user.Email, user.Role, user.Id);

            user.UpdateToken(request.RefreshToken, authResponseDto.RefreshToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return authResponseDto;
        }
    }
}
