namespace Dynamiq.API.DAL.Models
{
    public class Subscription
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        public Product Product { get; set; }
        public User User { get; set; }
    }
}
