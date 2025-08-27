using Dynamiq.Application.DTOs.PaymentDTOs;
using MediatR;

namespace Dynamiq.Application.Queries.Coupons.Queries
{
    public record class GetCouponByCodeQuery(string Code) : IRequest<CouponDto>;
}
