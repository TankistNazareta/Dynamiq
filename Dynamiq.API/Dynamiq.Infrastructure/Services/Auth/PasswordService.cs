using Dynamiq.Application.Interfaces.Auth;

namespace Dynamiq.Infrastructure.Services.Auth
{
    public class PasswordService : IPasswordService
    {
        public bool Check(string passHash, string pass) => BCrypt.Net.BCrypt.Verify(pass, passHash);
        public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    }
}
