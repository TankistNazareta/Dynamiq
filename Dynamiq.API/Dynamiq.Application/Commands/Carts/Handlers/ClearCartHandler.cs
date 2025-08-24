using Dynamiq.Application.Commands.Carts.Commands;
using Dynamiq.Application.Interfaces.Repositories;
using MediatR;

namespace Dynamiq.Application.Commands.Carts.Handlers
{
    internal class ClearCartHandler : IRequestHandler<ClearCartCommand>
    {
        private readonly ICartRepo _cartRepo;
        private readonly IUnitOfWork _unitOfWork;

        public ClearCartHandler(ICartRepo cartRepo, IUnitOfWork unitOfWork)
        {
            _cartRepo = cartRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepo.GetByUserIdAsync(request.UserId, cancellationToken);

            if (cart == null)
                throw new KeyNotFoundException($"Cart with user id: {request.UserId} wasn't found");

            _cartRepo.Remove(cart);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
