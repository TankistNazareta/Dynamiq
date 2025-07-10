using Dynamiq.API.Extension.Interfaces;
using Dynamiq.API.Extension.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Dynamiq.API.Extension.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _emailFrom = "youtopak@gmail.com";
        private readonly string _emailPassword = "dvrdupwfhqwjgwqr";
        private readonly string _smtpHost = "smtp.gmail.com";
        private readonly int _smtpPort = 587;

        public EmailService(
            //IOptions<EmailSettings> options
            )
        {
            //_emailFrom = options.Value.EmailFrom;
            //_emailPassword = options.Value.EmailPassword;
            //_smtpHost = options.Value.SmtpHost;
            //_smtpPort = options.Value.SmtpPort;
        }

        public async Task SendEmail(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Dynamiq official", _emailFrom));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailFrom, _emailPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
