using Dynamiq.API.Mapping.DTOs;
using Dynamiq.API.Stripe.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dynamiq.API.Controllers
{
    [Route("paymentHistory")]
    [ApiController]
    public class PaymentHistoryController : ControllerBase
    {
        private readonly IPaymentHistoryRepo _repo;
        private readonly ILogger<PaymentHistoryController> _logger;

        public PaymentHistoryController(IPaymentHistoryRepo repo, ILogger<PaymentHistoryController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _repo.GetAll();

            _logger.LogInformation("Retrieved all Payment History records, count: {Count}", all?.Count() ?? -1);

            return Ok(all);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _repo.GetById(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PaymentHistoryDto paymentHistory)
        {
            var response = await _repo.Insert(paymentHistory);

            _logger.LogInformation("Payment History with id {Id} was posted", response.Id);

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await _repo.Delete(id);

            _logger.LogInformation("Payment History with id {Id} was removed", id);

            return Ok("Payment history was removed");
        }
    }
}
