using Dynamiq.API.Extension.DTOs;
using Dynamiq.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dynamiq.API.Controllers
{
    [Route("/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repo;

        public UserController(IUserRepository repo)
        {
            _repo = repo;
        }

        [HttpPut("")]
        public async Task<IActionResult> Put([FromBody] UserDto user)
        {
            try
            {
                return Ok(await _repo.Update(user));
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

        [HttpGet("email")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            try
            {
                return Ok(await _repo.GetByEmail(email));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("id")]
        public async Task<IActionResult> GetById(Guid id)
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
        public async Task<IActionResult> Post([FromBody] UserDto user)
        {
            try
            {
                return Ok(await _repo.Insert(user));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _repo.Delete(id);
                return Ok("User was removed");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
