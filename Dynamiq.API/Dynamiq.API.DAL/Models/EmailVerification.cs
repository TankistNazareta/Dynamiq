using Microsoft.EntityFrameworkCore;

namespace Dynamiq.API.DAL.Models
{
    [Index(nameof(Token), IsUnique = true)]
    public class EmailVerification
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool ConfirmedEmail { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
