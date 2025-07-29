using AutoMapper;
using Dynamiq.Application.Commands.Carts.Commands;
using Dynamiq.Application.DTOs;
using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Interfaces.Repositories;
using MediatR;

namespace Dynamiq.Application.Commands.Carts.Handlers
{
    public class AddItemToCartHandler : IRequestHandler<AddItemToCartCommand, CartDto>
    {
        private readonly ICartRepo _cartRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AddItemToCartHandler(ICartRepo cartRepo, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _cartRepo = cartRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CartDto> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepo.GetByUserIdAsync(request.UserId, cancellationToken);

            if (cart == null)
            {
                cart = new Cart(request.UserId);
                cart.AddItem(request.ProductId, request.Quantity);
                await _cartRepo.AddAsync(cart, cancellationToken);
            }
            else
            {
                cart.AddItem(request.ProductId, request.Quantity);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<CartDto>(cart);
        }
    }
}
