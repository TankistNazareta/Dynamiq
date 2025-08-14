namespace Dynamiq.Application.Interfaces.Auth
{
    public interface IPasswordService
    {
        const string DefaultHashForOidc = "oidc-connect";
        bool Check(string passHash, string pass);
        string HashPassword(string password);
    }
}
