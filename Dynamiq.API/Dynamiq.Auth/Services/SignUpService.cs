using Dynamiq.API.Extension.Enums;
using Dynamiq.API.Mapping.DTOs;
using Dynamiq.Auth.DTOs;
using System.Text.Json;
using System.Text;
using Dynamiq.Auth.Interfaces;
using Dynamiq.API.Extension.Interfaces;

namespace Dynamiq.Auth.Services
{
    public class SignUpService : ISignUpService
    {
        private readonly HttpClient _apiClient;

        private readonly IEmailService _emailService;

        public SignUpService(IHttpClientFactory apiClient, IEmailService emailService)
        {
            _apiClient = apiClient.CreateClient("ApiClient");

            _emailService = emailService;
        }

        public async Task SignUp(AuthUserDto authUser)
        {
            var user = new UserDto()
            {
                Email = authUser.Email,
                PasswordHash = HashPassword(authUser.Password),
                Role = RoleEnum.User,
            };

            var emailVerify = new EmailVerificationDto()
            {
                UserId = user.Id
            };

            var userContent = new StringContent(
                JsonSerializer.Serialize(user),
                Encoding.UTF8,
                "application/json");

            var emailVerifyContent = new StringContent(
                JsonSerializer.Serialize(emailVerify),
                Encoding.UTF8,
                "application/json");

            var responseUser = await _apiClient.PostAsync("/users", userContent);

            if (!responseUser.IsSuccessStatusCode)
                throw new Exception($"Failed to create user: {responseUser.Content}");

            var responseEmailVerification = await _apiClient.PostAsync("/emailVerification", emailVerifyContent);

            if (!responseEmailVerification.IsSuccessStatusCode)
                throw new Exception($"Failed to create session for email: {responseEmailVerification.Content}");

            await _emailService.SendEmail(authUser.Email, "Confirm your email", GetHtmlBodyForSignUp(emailVerify.Token));
        }

        private string GetHtmlBodyForSignUp(string token) => $@"
        <body style=""font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;"">
            <div style=""max-width: 600px; margin: auto; background-color: #ffffff; padding: 30px; border-radius: 8px; box-shadow: 0 0 10px rgba(0,0,0,0.1);"">
                <h2 style=""color: #333;"">Confirm Your Email Address</h2>
                <p style=""font-size: 16px; color: #555;"">
                    Thank you for signing up. Please confirm your email address by clicking the button below. (You have 1 hour to confirm your email)
                </p>
                <a href=""https://yourdomain.com/confirm?token={token}""
                   style=""display: inline-block; padding: 12px 24px; margin-top: 20px; background-color: #007BFF; color: #fff; text-decoration: none; border-radius: 5px;"">
                    Confirm Email
                </a>
                <p style=""font-size: 14px; color: #888; margin-top: 30px;"">
                    If you did not create an account, you can safely ignore this email.
                </p>
            </div>
        </body>";

        private string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    }
}
