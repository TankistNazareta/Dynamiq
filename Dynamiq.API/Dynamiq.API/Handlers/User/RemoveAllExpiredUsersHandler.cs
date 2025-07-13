using Dynamiq.API.Commands.User;
using Dynamiq.API.DAL.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dynamiq.API.Handlers.User
{
    public class RemoveAllExpiredUsersHandler : IRequestHandler<RemoveAllExpiredUsersCommand, int>
    {
        private readonly AppDbContext _db;

        public RemoveAllExpiredUsersHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<int> Handle(RemoveAllExpiredUsersCommand request, CancellationToken cancellationToken)
        {
            var expiredUsers = await _db.Users
                .Include(u => u.EmailVerification)
                .Where(u =>
                u.EmailVerification.ExpiresAt < DateTime.UtcNow &&
                !u.EmailVerification.ConfirmedEmail)
                .ToListAsync();

            if (expiredUsers.Count == 0)
                return 0;

            _db.Users.RemoveRange(expiredUsers);
            await _db.SaveChangesAsync();

            return expiredUsers.Count;
        }
    }
}
