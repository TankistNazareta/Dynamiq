using Dynamiq.Application.Commands.Users.Commands;
using Dynamiq.Application.Interfaces.Auth;
using Dynamiq.Application.Interfaces.Repositories;
using MediatR;

namespace Dynamiq.Application.Commands.Users.Handlers
{
    public class ChangeUserPasswordHandler : IRequestHandler<ChangeUserPasswordCommand>
    {
        private readonly IUserRepo _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;

        public ChangeUserPasswordHandler(IUserRepo userRepo, IUnitOfWork unitOfWork, IPasswordService passwordService)
        {
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
            _passwordService = passwordService;
        }

        public async Task Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email, cancellationToken);

            if (user == null)
                throw new KeyNotFoundException("user with this email doesn't exist");

            if (!_passwordService.Check(user.PasswordHash, request.OldPassword))
                throw new ArgumentException("your old password isn't correct");

            user.ChangePassword(_passwordService.HashPassword(request.NewPassword));

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
