namespace Dynamiq.API.Extension.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        /// Sends an HTML email message to the specified recipient.
        /// </summary>
        /// <param name="toEmail">The recipient's email address.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The HTML content of the email.</param>
        /// <remarks>
        /// ⚠️ Consider applying a rate limiter on the controller that calls this method
        /// to prevent abuse or email spam.
        /// </remarks>
        Task SendEmail(string toEmail, string subject, string body);
    }
}
