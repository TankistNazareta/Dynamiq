using Dynamiq.Application.Commands.Users.Commands;
using Dynamiq.Application.Interfaces.Auth;
using Dynamiq.Domain.Entities;
using Dynamiq.Domain.Enums;
using Dynamiq.Domain.Interfaces.Repositories;
using Dynamiq.Domain.Enums;
using MediatR;

namespace Dynamiq.Application.Commands.Users.Handlers
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand>
    {
        private readonly IUserRepo _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;

        public RegisterUserHandler(IUserRepo userRepo, IUnitOfWork unitOfWork, IPasswordService passwordService)
        {
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
            _passwordService = passwordService;
        }

        public async Task Handle(RegisterUserCommand request, CancellationToken ct)
        {
            var existingUser = await _userRepo.GetByEmailAsync(request.Email, ct);

            if (existingUser != null)
                throw new ArgumentException("User with this email is already exist");

            var hashedPassword = _passwordService.HashPassword(request.Password);

            var user = new User(request.Email, hashedPassword, RoleEnum.DefaultUser);

            await _userRepo.AddAsync(user, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
