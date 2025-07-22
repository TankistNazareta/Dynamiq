namespace Dynamiq.Infrastructure.Settings
{
    public class EmailSettings
    {
        public string EmailFrom { get; set; } = string.Empty;
        public string EmailPassword { get; set; } = string.Empty;
        public string SmtpHost { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
    }
}
