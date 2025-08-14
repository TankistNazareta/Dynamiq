using Dynamiq.Application.Commands.Users.Commands;
using Dynamiq.Application.Common;
using Dynamiq.Application.CustomExceptions;
using Dynamiq.Application.DTOs.AuthDTOs;
using Dynamiq.Application.Interfaces.Auth;
using Dynamiq.Application.Interfaces.Services;
using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Exceptions;
using Dynamiq.Domain.Interfaces.Repositories;
using MediatR;

namespace Dynamiq.Application.Commands.Users.Handlers
{
    public class LogInUserHandler : IRequestHandler<LogInUserCommand, AuthResponseDto>
    {
        private readonly IUserRepo _userRepo;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public LogInUserHandler(
            IUserRepo userRepo,
            IPasswordService passwordService,
            ITokenService tokenService,
            IUnitOfWork unitOfWork,
            IEmailService emailService)
        {
            _passwordService = passwordService;
            _userRepo = userRepo;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<AuthResponseDto> Handle(LogInUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email, cancellationToken);

            if (user == null)
                throw new KeyNotFoundException("User with this email wasn't found");
            if (!user.EmailVerification.IsConfirmed)
                throw new EmailNotConfirmedException(user.Email);
            if (user.PasswordHash == IPasswordService.DefaultHashForOidc)
                throw new CannotLinkOidcAccountException(user.Email);
            if (!_passwordService.Check(user.PasswordHash, request.Password))
                throw new ArgumentException("Your password isn't correct");

            var authResponseDto = _tokenService.CreateAuthResponse(user.Email, user.Role, user.Id);

            user.AddRefreshToken(new RefreshToken(user.Id, authResponseDto.RefreshToken));

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _emailService.SendEmailAsync(user.Email, "New log in", HtmlBodyForEmail.GetLogInBody());

            return authResponseDto;
        }
    }
}
