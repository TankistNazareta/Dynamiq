namespace Dynamiq.API.Mapping.DTOs
{
    public class SubscriptionDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        public ProductDto? Product { get; set; } = null!;
        public UserDto? User { get; set; } = null!;
    }
}
