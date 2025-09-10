using AutoMapper;
using Dynamiq.Application.Commands.Carts.Commands;
using Dynamiq.Application.DTOs.AccountDTOs;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Domain.Aggregates;
using MediatR;

namespace Dynamiq.Application.Commands.Carts.Handlers
{
    public class SetQuantityCartItemHandler : IRequestHandler<SetQuantityCartItemCommand, CartDto>
    {
        private readonly ICartRepo _cartRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SetQuantityCartItemHandler(ICartRepo cartRepo, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _cartRepo = cartRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CartDto> Handle(SetQuantityCartItemCommand request, CancellationToken cancellationToken)
        {
            var isNewCart = false;
            var cart = await _cartRepo.GetByUserIdAsync(request.UserId, cancellationToken);

            if (cart == null)
            {
                cart = new Cart(request.UserId);
                isNewCart = true;
            }

            cart.SetItemQuantity(request.ProductId, request.Quantity);

            if (isNewCart)
                await _cartRepo.AddAsync(cart, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<CartDto>(cart);
        }
    }
}
