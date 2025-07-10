using Dynamiq.API.Interfaces;
using Dynamiq.API.Mapping.DTOs;
using Dynamiq.API.Stripe.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dynamiq.API.Controllers
{
    [Route("/subscriptions")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionRepo _repo;

        public SubscriptionController(ISubscriptionRepo repo)
        {
            _repo = repo;
        }

        [HttpPut("")]
        public async Task<IActionResult> Put([FromBody] SubscriptionDto subscription)
        {
            try
            {
                return Ok(await _repo.Update(subscription));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await _repo.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            try
            {
                return Ok(await _repo.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("")]
        public async Task<IActionResult> Post([FromBody] SubscriptionDto subscription)
        {
            try
            {
                return Ok(await _repo.Insert(subscription));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                await _repo.Delete(id);
                return Ok("SubscriptionData was removed");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
