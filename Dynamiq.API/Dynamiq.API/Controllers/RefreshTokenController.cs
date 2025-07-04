using Dynamiq.API.Interfaces;
using Dynamiq.API.Mapping.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Dynamiq.API.Controllers
{
    [Route("/refresh")]
    public class RefreshTokenController : ControllerBase
    {
        private readonly IRefreshTokenRepo _repo;

        public RefreshTokenController(IRefreshTokenRepo repo)
        {
            _repo = repo;
        }

        [HttpPost("")]
        public async Task<IActionResult> Post([FromBody] RefreshTokenDto token)
        {
            try
            {
                await _repo.Insert(token);
                return Ok("Token successfully posted");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{token}")]
        public async Task<IActionResult> GetByToken([FromQuery] string token)
        {
            try
            {
                
                return Ok(await _repo.GetByToken(token));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("userId")]
        public async Task<IActionResult> GetByUserId([FromQuery] Guid userId)
        {
            try
            {
                return Ok(await _repo.GetByUserId(userId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("revoke")]
        public async Task<IActionResult> Revoke([FromQuery] string token)
        {
            try
            {
                await _repo.Revoke(token);
                return Ok("Token successfully revoked");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("")]
        public async Task<IActionResult> Put([FromBody] RefreshTokenDto token)
        {
            try
            {
                await _repo.Update(token);
                return Ok("Token successfully updated");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
