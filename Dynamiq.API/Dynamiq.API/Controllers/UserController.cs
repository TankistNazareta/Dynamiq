using Dynamiq.API.Interfaces;
using Dynamiq.API.Mapping.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dynamiq.API.Controllers
{
    [Route("users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepo _repo;
        private readonly ILogger _logger;

        public UserController(IUserRepo repo, ILogger<UserController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _repo.GetAll();

            _logger.LogInformation("Retrieved all users, count: {Count}", all?.Count() ?? -1);

            return Ok(all);
        }

        [HttpGet("email")]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            var user = await _repo.GetByEmail(email);

            _logger.LogInformation("Retrieved user by email: {Email}", email);

            return Ok(user);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var user = await _repo.GetById(id);

            _logger.LogInformation("Retrieved user with id: {Id}", id);

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserDto user)
            {
            var created = await _repo.Insert(user);

            _logger.LogInformation("Created user with id: {Id}", created.Id);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UserDto user)
        {
            var updated = await _repo.Update(user);

            _logger.LogInformation("Updated user with id: {Id}", user.Id);

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await _repo.Delete(id);

            _logger.LogInformation("Deleted user with id: {Id}", id);

            return Ok("User was removed");
        }
    }
}
