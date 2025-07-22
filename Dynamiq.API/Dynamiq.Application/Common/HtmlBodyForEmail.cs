namespace Dynamiq.Application.Common
{
    public static class HtmlBodyForEmail
    {
        public static string GetLogInBody() => $@"
            <body style=""font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;"">
                <div style=""max-width: 600px; margin: auto; background-color: #ffffff; padding: 30px; border-radius: 8px; box-shadow: 0 0 10px rgba(0,0,0,0.1);"">
                    <h2 style=""color: #333;"">New Sign-In to Your Account</h2>
                    <p style=""font-size: 16px; color: #555;"">
                        We noticed a new log-in to your account. If this was you, you can safely ignore this message.
                    </p>
                    <p style=""font-size: 16px; color: #555;"">
                        If this wasn't you, please secure your account immediately by changing your password.
                    </p>
                    <a href=""https://yourdomain.com/account/security""
                       style=""display: inline-block; padding: 12px 24px; margin-top: 20px; background-color: #DC3545; color: #fff; text-decoration: none; border-radius: 5px;"">
                        Secure My Account
                    </a>
                </div>
            </body>";

        public static string GetSignInBody(string token) => $@"
        <body style=""font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;"">
            <div style=""max-width: 600px; margin: auto; background-color: #ffffff; padding: 30px; border-radius: 8px; box-shadow: 0 0 10px rgba(0,0,0,0.1);"">
                <h2 style=""color: #333;"">Confirm Your Email Address</h2>
                <p style=""font-size: 16px; color: #555;"">
                    Thank you for signing up. Please confirm your email address by clicking the button below. (You have 1 hour to confirm your email)
                </p>
                <a href=""https://yourdomain.com/confirm?token={token}""
                   style=""display: inline-block; padding: 12px 24px; margin-top: 20px; background-color: #007BFF; color: #fff; text-decoration: none; border-radius: 5px;"">
                    Confirm Email
                </a>
                <p style=""font-size: 14px; color: #888; margin-top: 30px;"">
                    If you did not create an account, you can safely ignore this email.
                </p>
            </div>
        </body>";
    }
}
