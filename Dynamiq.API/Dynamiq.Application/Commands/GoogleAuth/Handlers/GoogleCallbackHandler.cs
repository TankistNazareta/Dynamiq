using Dynamiq.Application.Commands.GoogleAuth.Commands;
using Dynamiq.Application.Commands.RefreshTokens.Commands;
using Dynamiq.Application.Common;
using Dynamiq.Application.DTOs.AuthDTOs;
using Dynamiq.Application.Interfaces.Auth;
using Dynamiq.Application.Interfaces.Services;
using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Enums;
using Dynamiq.Domain.Exceptions;
using Dynamiq.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Dynamiq.Application.Commands.GoogleAuth.Handlers
{
    public class GoogleCallbackHandler : IRequestHandler<GoogleCallbackCommand, AuthTokensDto>
    {
        private readonly IGoogleOidcService _google;
        private readonly IUserRepo _userRepo;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public GoogleCallbackHandler(
            IGoogleOidcService google,
            IUserRepo userRepo,
            ITokenService tokens,
            IHttpContextAccessor httpContext,
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IEmailService emailService)
        {
            _google = google;
            _userRepo = userRepo;
            _tokenService = tokens;
            _httpContext = httpContext;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<AuthTokensDto> Handle(GoogleCallbackCommand request, CancellationToken ct)
        {
            var cookies = _httpContext.HttpContext!.Request.Cookies;

            if (cookies.TryGetValue("g_state", out var expectedState) && !string.IsNullOrEmpty(request.State))
            {
                if (!CryptographicOperations.FixedTimeEquals(
                        Encoding.UTF8.GetBytes(request.State),
                        Encoding.UTF8.GetBytes(expectedState)))
                {
                    throw new InvalidOperationException("Invalid state");
                }
            }

            var tokens = await _google.ExchangeCodeAsync(request.Code, ct);

            var (principal, _) = await _google.ValidateIdTokenAsync(tokens.id_token, ct);

            var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email || c.Type == "email")?.Value
                        ?? throw new InvalidOperationException("Google did not return email. Ensure scope includes 'email'.");

            var emailVerified = principal.Claims.FirstOrDefault(c => c.Type == "email_verified")?.Value;

            var user = await _userRepo.GetByEmailAsync(email, ct);

            if (user == null)
            {
                user = new User(email, IPasswordService.DefaultHashForOidc, RoleEnum.DefaultUser);

                await _userRepo.AddAsync(user, ct);
                await _unitOfWork.SaveChangesAsync(ct);

                user.EmailVerification.Confirm(email);
            }
            else if(user.PasswordHash != IPasswordService.DefaultHashForOidc)
            {
                throw new CannotLinkOidcAccountException(email);
            }

            var authResponseDto = _tokenService.CreateAuthResponse(user.Email, user.Role, user.Id);
            user.AddRefreshToken(new RefreshToken(user.Id, authResponseDto.RefreshToken));

            await _unitOfWork.SaveChangesAsync(ct);

            await _emailService.SendEmailAsync(user.Email, "New log in", HtmlBodyForEmail.GetLogInBody());

            return authResponseDto;
        }
    }
}
