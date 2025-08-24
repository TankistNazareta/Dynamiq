using Dynamiq.Application.Commands.Users.Commands;
using Dynamiq.Application.Interfaces.Repositories;
using MediatR;

namespace Dynamiq.Application.Commands.Users.Handlers
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly IUserRepo _userRepo;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUserHandler(IUserRepo userRepo, IUnitOfWork unitOfWork)
        {
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteUserCommand request, CancellationToken ct)
        {
            var user = await _userRepo.GetByIdAsync(request.Id, ct);

            if (user == null)
                throw new KeyNotFoundException("DefaultUser with this Id doesn't exists");

            _userRepo.Delete(user);

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
