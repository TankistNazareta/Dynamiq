using AutoMapper;
using Dynamiq.Application.DTOs.PaymentDTOs;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Application.Queries.Coupons.Queries;
using MediatR;

namespace Dynamiq.Application.Queries.Coupons.Handlers
{
    public class GetCouponByCodeHandler : IRequestHandler<GetCouponByCodeQuery, CouponDto>
    {
        private readonly ICouponRepo _couponRepo;
        private readonly IMapper _mapper;

        public GetCouponByCodeHandler(ICouponRepo couponRepo, IMapper mapper)
        {
            _couponRepo = couponRepo;
            _mapper = mapper;
        }

        public async Task<CouponDto> Handle(GetCouponByCodeQuery request, CancellationToken cancellationToken)
        {
            var coupon = await _couponRepo.GetByCodeAsync(request.Code, cancellationToken);

            if (coupon == null)
                throw new KeyNotFoundException("Cupon wasn't found");

            return _mapper.Map<CouponDto>(coupon);
        }
    }
}
