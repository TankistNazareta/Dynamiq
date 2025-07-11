namespace Dynamiq.API.Mapping.DTOs
{
    public class RefreshTokenDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false;
        public Guid UserId { get; set; }
        public UserDto? User { get; set; } = null;
    }
}
