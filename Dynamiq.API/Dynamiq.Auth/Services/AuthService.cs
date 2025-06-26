using Dynamiq.API.Extension.DTOs;
using Dynamiq.API.Extension.Enums;
using Dynamiq.Auth.DTOs;
using Dynamiq.Auth.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Dynamiq.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _apiClient;

        public AuthService(IConfiguration config, IHttpClientFactory apiClient)
        {
            _config = config;
            _apiClient = apiClient.CreateClient("ApiClient");
        }

        public async Task<AuthResponseDto> LogIn(AuthUserDto authUser)
        {
            var user = await _apiClient.GetFromJsonAsync<UserDto>($"/users/email?email={authUser.Email}");

            if (user == null)
                throw new ArgumentException("User not found");

            if (!CheckPassword(authUser.Password, user.PasswordHash))
                throw new ArgumentException("Incorrect password");

            return await PostAndCreateAuthResponseDto(user);
        }

        public async Task<AuthResponseDto> SignUp(AuthUserDto authUser)
        {
            var user = new UserDto()
            {
                Email = authUser.Email,
                PasswordHash = HashPassword(authUser.Password),
                Role = RoleEnum.User,
                ConfirmedEmail = false
            };

            var content = new StringContent(
                JsonSerializer.Serialize(user),
                Encoding.UTF8,
                "application/json");

            var response = await _apiClient.PostAsync("/users", content);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to create user: {response.Content}");

            return await PostAndCreateAuthResponseDto(user);
        }

        public async Task<AuthResponseDto> Refresh(string token)
        {
            // Get token
            var responseToken = await _apiClient.GetFromJsonAsync<RefreshTokenDto>($"/refresh/{token}")
                ?? throw new Exception("Failed to refresh token: token not found");

            //Check token
            if (!responseToken.IsRevoked)
                throw new ArgumentException("Your token is revoked");

            if (responseToken.ExpiresAt < DateTime.UtcNow)
                throw new ArgumentException("Your token has expired");

            // Revoke old token
            var responsePut = await _apiClient.PutAsync($"/refresh/{token}", null);
            if (!responsePut.IsSuccessStatusCode)
                throw new Exception("Failed to revoke token");

            var authResponseDto = await PostAndCreateAuthResponseDto(new()
            {
                Id = responseToken.User.Id,
                Email = responseToken.User.Email,
                Role = responseToken.User.Role,
            }, responseToken.ExpiresAt);

            return authResponseDto;
        }

        public async Task Revoke(string token)
        {
            var response = await _apiClient.PutAsync($"/refresh/{token}", null);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to revoke new refresh token");
        }

        private async Task<AuthResponseDto> PostAndCreateAuthResponseDto(UserDto user, DateTime? expiresAt = null)
        {
            var accessToken = GenerateJwtToken(user.Email, user.Role);
            var refreshToken = GenerateRefreshToken(user.Id, expiresAt);

            var refreshTokenJson = new StringContent(
                JsonSerializer.Serialize(refreshToken),
                Encoding.UTF8,
                "application/json");

            var response = await _apiClient.PostAsync("/refresh", refreshTokenJson);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to Log in: {response.Content}");

            return new()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
        }

        private string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
        private bool CheckPassword(string password, string HashedPassword)
            => BCrypt.Net.BCrypt.Verify(password, HashedPassword);

        private string GenerateJwtToken(string email, RoleEnum role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private RefreshTokenDto GenerateRefreshToken(Guid userId, DateTime? expiresAt)
        {
            var bytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);

            var refreshTokenString = Convert.ToBase64String(bytes);

            return new()
            {
                Token = refreshTokenString,
                UserId = userId,
                ExpiresAt = expiresAt ?? DateTime.UtcNow.AddDays(7)
            };
        }
    }
}
