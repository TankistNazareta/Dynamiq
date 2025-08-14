namespace Dynamiq.Application.DTOs.AuthDTOs
{
    public class GoogleOAuthOptions
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty; // e.g. https://localhost:5001/auth/google/callback
    }
}
