namespace Dynamiq.Application.DTOs.AuthDTOs
{
    public class GoogleTokenResponse
    {
        public string access_token { get; set; } = string.Empty;
        public int expires_in { get; set; }
        public string refresh_token { get; set; } = string.Empty; // Google refresh (optional, usually not needed)
        public string scope { get; set; } = string.Empty;
        public string token_type { get; set; } = string.Empty;
        public string id_token { get; set; } = string.Empty; // contains email, sub, etc.
    }
}
