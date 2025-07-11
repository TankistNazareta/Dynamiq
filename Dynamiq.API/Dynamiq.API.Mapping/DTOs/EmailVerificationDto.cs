namespace Dynamiq.API.Mapping.DTOs
{
    public class EmailVerificationDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Token { get; set; } = Guid.NewGuid().ToString();
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddHours(1);
        public bool ConfirmedEmail { get; set; } = false;
        public Guid UserId { get; set; }
        public UserDto? User { get; set; } = null!;
    }
}
