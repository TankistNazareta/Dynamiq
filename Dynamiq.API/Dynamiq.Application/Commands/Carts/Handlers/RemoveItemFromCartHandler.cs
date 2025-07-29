using AutoMapper;
using Dynamiq.Application.Commands.Carts.Commands;
using Dynamiq.Application.DTOs;
using Dynamiq.Domain.Interfaces.Repositories;
using MediatR;

namespace Dynamiq.Application.Commands.Carts.Handlers
{
    internal class RemoveItemFromCartHandler : IRequestHandler<RemoveItemFromCartCommand, CartDto>
    {
        private readonly ICartRepo _cartRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RemoveItemFromCartHandler(ICartRepo cartRepo, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _cartRepo = cartRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CartDto> Handle(RemoveItemFromCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepo.GetByUserIdAsync(request.UserId, cancellationToken);

            if (cart == null)
                throw new KeyNotFoundException($"Cart with user id: {request.UserId} wasn't found");

            cart.RemoveItem(request.ProductId, request.Quantity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<CartDto>(cart);
        }
    }
}
