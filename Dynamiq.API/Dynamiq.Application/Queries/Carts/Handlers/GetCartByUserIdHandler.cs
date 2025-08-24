using AutoMapper;
using Dynamiq.Application.DTOs.AccountDTOs;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Application.Queries.Carts.Queries;
using MediatR;

namespace Dynamiq.Application.Queries.Carts.Handlers
{
    public class GetCartByUserIdHandler : IRequestHandler<GetCartByUserIdQuery, CartDto?>
    {
        private readonly IMapper _mapper;
        private readonly ICartRepo _cartRepo;

        public GetCartByUserIdHandler(IMapper mapper, ICartRepo cartRepo)
        {
            _mapper = mapper;
            _cartRepo = cartRepo;
        }

        public async Task<CartDto?> Handle(GetCartByUserIdQuery request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepo.GetByUserIdAsync(request.UserId, cancellationToken);

            return _mapper.Map<CartDto?>(cart);
        }
    }
}
