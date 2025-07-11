using Dynamiq.API.Interfaces;
using Dynamiq.API.Mapping.DTOs;
using Dynamiq.API.Stripe.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dynamiq.API.Controllers
{
    [Route("subscriptions")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionRepo _repo;
        private readonly ILogger _logger;

        public SubscriptionController(ISubscriptionRepo repo, ILogger<SubscriptionController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _repo.GetAll();

            _logger.LogInformation("Retrieved all subscriptions, count: {Count}", all?.Count() ?? -1);

            return Ok(all);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var subscription = await _repo.GetById(id);

            _logger.LogInformation("Retrieved subscription with id: {Id}", id);

            return Ok(subscription);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SubscriptionDto subscription)
        {
            var created = await _repo.Insert(subscription);

            _logger.LogInformation("Created subscription with id: {Id}", created.Id);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] SubscriptionDto subscription)
        {
            var updated = await _repo.Update(subscription);

            _logger.LogInformation("Updated subscription with id: {Id}", subscription.Id);

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await _repo.Delete(id);

            _logger.LogInformation("Deleted subscription with id: {Id}", id);

            return Ok("Subscription was removed");
        }
    }
}
