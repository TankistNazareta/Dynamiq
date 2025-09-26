using Dynamiq.Application.Queries.PaymentHistory.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dynamiq.API.Controllers
{
    [Route("api/payment-history")]
    [ApiController]
    public class PaymentHistoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentHistoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("by-id")]
        public async Task<IActionResult> GetById([FromQuery] Guid id)
        {
            var paymentHistory = await _mediator.Send(new GetPaymentHistoriesByUserIdQuery(id));

            return Ok(paymentHistory);
        }

        [HttpGet("by-email")]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            var paymentHistory = await _mediator.Send(new GetPaymentHistoriesByUserEmailQuery(email));

            return Ok(paymentHistory);
        }
    }
}
