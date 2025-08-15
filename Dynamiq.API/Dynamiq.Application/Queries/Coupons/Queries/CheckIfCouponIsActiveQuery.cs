using MediatR;

namespace Dynamiq.Application.Queries.Coupons.Queries
{
    public record class CheckIfCouponIsActiveQuery(string Code) : IRequest<bool>;
}
