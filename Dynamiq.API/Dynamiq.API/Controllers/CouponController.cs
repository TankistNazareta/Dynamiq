using Dynamiq.Application.Queries.Coupons.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dynamiq.API.Controllers
{
    [ApiController]
    [Route("coupon")]
    public class CouponController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CouponController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetCouponByCode([FromQuery] string code)
        {
            var coupon = await _mediator.Send(new GetCouponByCodeQuery(code));

            return Ok(coupon);
        }
    }
}
