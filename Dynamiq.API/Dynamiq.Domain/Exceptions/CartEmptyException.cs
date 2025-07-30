namespace Dynamiq.Domain.Exceptions
{
    public class CartEmptyException : DomainException
    {
        public CartEmptyException()
            : base("Cart cannot be empty.") { }

        public CartEmptyException(string message)
            : base(message) { }

        public CartEmptyException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
