using Dynamiq.API.Interfaces;
using Dynamiq.API.Mapping.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Dynamiq.API.Controllers
{
    [Route("/emailVerification")]
    public class EmailVerificationController : ControllerBase
    {
        private readonly IEmailVerificationRepo _emailRepo;

        public EmailVerificationController(IEmailVerificationRepo emailRepo)
        {
            _emailRepo = emailRepo;
        }

        [HttpPut("")]
        public async Task<IActionResult> Put([FromBody] EmailVerificationDto subscription)
        {
            try
            {
                return Ok(await _emailRepo.Update(subscription));
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
                return Ok(await _emailRepo.GetAll());
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
                return Ok(await _emailRepo.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("")]
        public async Task<IActionResult> Post([FromBody] EmailVerificationDto subscription)
        {
            try
            {
                return Ok(await _emailRepo.Insert(subscription));
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
                await _emailRepo.Delete(id);
                return Ok("SubscriptionData was removed");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] Guid id)
        {
            try
            {
                await _emailRepo.ConfirmEmail(id);
                return Ok("You confirmed email successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("token/{token}")]
        public async Task<IActionResult> GetByToken([FromQuery] string token)
        {
            try
            {
                return Ok(await _emailRepo.GetByToken(token));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
