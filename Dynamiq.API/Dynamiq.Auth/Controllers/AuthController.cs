using Dynamiq.Auth.DTOs;
using Dynamiq.Auth.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dynamiq.Auth.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthUserDto authUser)
        {
            try
            {
                var token = await _authService.Login(authUser);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] AuthUserDto authUser)
        {
            try
            {
                var token = await _authService.Signup(authUser);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
