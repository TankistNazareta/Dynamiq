using Dynamiq.API.Extension.Interfaces;
using Dynamiq.API.Mapping.DTOs;
using Dynamiq.Auth.DTOs;
using Dynamiq.Auth.Interfaces;

namespace Dynamiq.Auth.Services
{
    public class LogInService : ILogInService
    {
        private readonly HttpClient _apiClient;

        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public LogInService(IHttpClientFactory apiClient, ITokenService tokenService, IEmailService emailService)
        {
            _apiClient = apiClient.CreateClient("ApiClient");

            _tokenService = tokenService;
            _emailService = emailService;
        }

        public async Task<AuthResponseDto> LogIn(AuthUserDto authUser)
        {
            var user = await _apiClient.GetFromJsonAsync<UserDto>($"/users/email?email={authUser.Email}");

            if (user == null)
                throw new ArgumentException("User not found");

            if (user.EmailVerification.ConfirmedEmail == false)
                throw new Exception("Please, confirm your email");

            //check password
            if (!BCrypt.Net.BCrypt.Verify(authUser.Password, user.PasswordHash))
                throw new ArgumentException("Incorrect password");

            var authResponse = await _tokenService.CreateAuthResponse(user);

            await _emailService.SendEmail(authUser.Email, "New Log In", GetHtmlCodeLogIn());

            return authResponse;
        }

        private string GetHtmlCodeLogIn() => $@"
            <body style=""font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;"">
                <div style=""max-width: 600px; margin: auto; background-color: #ffffff; padding: 30px; border-radius: 8px; box-shadow: 0 0 10px rgba(0,0,0,0.1);"">
                    <h2 style=""color: #333;"">New Sign-In to Your Account</h2>
                    <p style=""font-size: 16px; color: #555;"">
                        We noticed a new log-in to your account. If this was you, you can safely ignore this message.
                    </p>
                    <p style=""font-size: 16px; color: #555;"">
                        If this wasn't you, please secure your account immediately by changing your password.
                    </p>
                    <a href=""https://yourdomain.com/account/security""
                       style=""display: inline-block; padding: 12px 24px; margin-top: 20px; background-color: #DC3545; color: #fff; text-decoration: none; border-radius: 5px;"">
                        Secure My Account
                    </a>
                </div>
            </body>";
    }
}
