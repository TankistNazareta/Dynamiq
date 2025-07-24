using Dynamiq.Application.Interfaces.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Dynamiq.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger _logger;

        private readonly string _emailFrom;
        private readonly string _emailPassword;
        private readonly string _smtpHost;
        private readonly int _smtpPort;

        public EmailService(
            IConfiguration config,
            ILogger<EmailService> logger
            )
        {
            _emailFrom = config["Smtp:EmailFrom"]!;
            _emailPassword = config["Smtp:Password"]!;
            _smtpHost = config["Smtp:Host"]!;
            _smtpPort = int.Parse(config["Smtp:Port"]!);

            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
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

                _logger.LogInformation("mail was send to email: {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {ToEmail}", toEmail);
                throw;
            }
        }
    }
}
