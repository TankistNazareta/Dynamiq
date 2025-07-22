namespace Dynamiq.Application.Interfaces.Auth
{
    public interface IPasswordService
    {
        bool Check(string passHash, string pass);
        string HashPassword(string password);
    }
}
