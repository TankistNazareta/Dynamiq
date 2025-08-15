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

        [HttpGet("CheckIfActiveByCode")]
        public async Task<IActionResult> CheckIfActiveByCode(string code)
        {
            var isActive = await _mediator.Send(new CheckIfCouponIsActiveQuery(code));

            return Ok(isActive);
        }
    }
}
