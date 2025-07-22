using Dynamiq.Application.Interfaces.Services;
using Dynamiq.Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Dynamiq.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger _logger;

        private readonly string _emailFrom = "youtopak@gmail.com";
        private readonly string _emailPassword = "yeriznpcttczlobo";
        private readonly string _smtpHost = "smtp.gmail.com";
        private readonly int _smtpPort = 587;

        public EmailService(
            IOptions<EmailSettings> options,
            ILogger<EmailService> logger
            )
        {
            //_emailFrom = options.Value.EmailFrom;
            //_emailPassword = options.Value.EmailPassword;
            //_smtpHost = options.Value.SmtpHost;
            //_smtpPort = options.Value.SmtpPort;

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
