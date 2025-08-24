using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Application.Interfaces.UseCases;

namespace Dynamiq.Application.UseCases
{
    public class UserCleanupUseCase : IUserCleanupUseCase
    {
        private readonly IUserRepo _userRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UserCleanupUseCase(IUnitOfWork unitOfWork, IUserRepo userRepo)
        {
            _unitOfWork = unitOfWork;
            _userRepo = userRepo;
        }

        public async Task<int> RemoveAllExpiredUsersAsync(CancellationToken cancellationToken)
        {
            var expiredusers = await _userRepo.GetAllExpiredUsersAsync(cancellationToken);

            foreach (var expireduser in expiredusers)
                _userRepo.Delete(expireduser);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return expiredusers.Count;
        }
    }
}
