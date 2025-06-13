using Dynamiq.API.Extension.DTOs;
using Dynamiq.API.Extension.Enums;
using Dynamiq.Auth.DTOs;
using Dynamiq.Auth.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
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

        public async Task<string> Login(AuthUserDto authUser)
        {
            var user = await _apiClient.GetFromJsonAsync<UserDto>($"/users/email?email={authUser.Email}");

            if (user == null)
                throw new ArgumentException("User not found");

            if (!CheckPassword(authUser.Password, user.PasswordHash))
                throw new ArgumentException("Incorrect password");

            return GenerateJwtToken(authUser.Email, user.Role);
        }

        public async Task<string> Signup(AuthUserDto authUser)
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

            return GenerateJwtToken(authUser.Email, user.Role);
        }
    }
}
