using Dynamiq.Application.DTOs;
using Dynamiq.Application.Interfaces.Auth;
using Dynamiq.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Dynamiq.Infrastructure.Services.Auth
{
    public class TokenService : ITokenService
    {
        private readonly string _jwtKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;

        public TokenService(IConfiguration config)
        {
            _jwtKey = config["JwtSettings:Key"]!;
            _jwtIssuer = config["JwtSettings:Issuer"]!;
            _jwtAudience = config["JwtSettings:Audience"]!;
        }

        public AuthResponseDto CreateAuthResponse(string email, RoleEnum role)
        {
            var accessToken = GenerateJwtToken(email, role);
            var refreshToken = GenerateRefreshToken();

            return new(accessToken, refreshToken);
        }


        private string GenerateJwtToken(string email, RoleEnum role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var bytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);

            var refreshTokenString = Convert.ToBase64String(bytes);

            return refreshTokenString;
        }
    }
}
