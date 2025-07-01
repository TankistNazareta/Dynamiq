namespace Dynamiq.API.Payment.Interfaces
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentIntent(long amount, string currency);
        Task<bool> HandleWebhook(string json, string stripeSignature);
    }
}
