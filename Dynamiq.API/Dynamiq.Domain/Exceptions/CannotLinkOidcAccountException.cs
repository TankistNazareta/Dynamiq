namespace Dynamiq.Domain.Exceptions
{
    /// <summary>
    /// Thrown when a user attempts to link an OIDC account,
    /// but a user with the same email already exists. (or reverse)
    /// </summary>
    public class CannotLinkOidcAccountException : Exception
    {
        public string Email { get; }

        public CannotLinkOidcAccountException(string email)
            : base($"You had connected this email by another way")
        {
            Email = email;
        }
    }
}
