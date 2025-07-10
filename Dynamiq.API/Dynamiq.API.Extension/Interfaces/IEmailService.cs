namespace Dynamiq.API.Extension.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(string toEmail, string subject, string body);
    }
}
