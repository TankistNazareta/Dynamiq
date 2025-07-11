using Dynamiq.API.Interfaces;
using Dynamiq.API.Mapping.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Dynamiq.API.Controllers
{
    [Route("refresh")]
    [ApiController]
    public class RefreshTokenController : ControllerBase
    {
        private readonly IRefreshTokenRepo _repo;
        private readonly ILogger _logger;

        public RefreshTokenController(IRefreshTokenRepo repo, ILogger<RefreshTokenController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RefreshTokenDto token)
        {
            var created = await _repo.Insert(token);

            _logger.LogInformation("Refresh token created for UserId: {UserId}", created.UserId);

            return CreatedAtAction(nameof(GetByToken), new { token = created.Token }, created);
        }

        [HttpGet("{token}")]
        public async Task<IActionResult> GetByToken([FromRoute] string token)
        {
            var result = await _repo.GetByToken(token);

            _logger.LogInformation("Retrieved refresh token: {Token}", token);

            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId([FromRoute] Guid userId)
        {
            var result = await _repo.GetByUserId(userId);

            _logger.LogInformation("Retrieved refresh token for userId: {UserId}", userId);

            return Ok(result);
        }

        [HttpPut("revoke")]
        public async Task<IActionResult> Revoke([FromQuery] string token)
        {
            await _repo.Revoke(token);

            _logger.LogInformation("Refresh token revoked: {Token}", token);

            return Ok("Token successfully revoked");
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] RefreshTokenDto token)
        {
            await _repo.Update(token);

            _logger.LogInformation("Refresh token updated for UserId: {UserId}", token.UserId);

            return Ok("Token successfully updated");
        }
    }
}
