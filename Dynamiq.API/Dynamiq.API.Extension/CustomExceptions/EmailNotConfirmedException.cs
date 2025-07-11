namespace Dynamiq.API.Extension.CustomExceptions
{
    public class EmailNotConfirmedException : Exception
    {
        public string Email { get; private set; }

        public EmailNotConfirmedException(string email, string? exMsg = "Please, confirm your email.") 
            : base(exMsg) 
        { 
            Email = email;
        }
    }
}
