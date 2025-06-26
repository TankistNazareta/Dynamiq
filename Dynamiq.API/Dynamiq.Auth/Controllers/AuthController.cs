using Dynamiq.Auth.DTOs;
using Dynamiq.Auth.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dynamiq.Auth.Controllers
{
    [Route("/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromBody] AuthUserDto authUser)
        {
            try
            {
                var token = await _authService.LogIn(authUser);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] AuthUserDto authUser)
        {
            try
            {
                var token = await _authService.SignUp(authUser);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string token)
        {
            try
            {
                var tokens = await _authService.Refresh(token);
                return Ok(tokens);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("revoke")]
        public async Task<IActionResult> Revoke([FromBody] string token)
        {
            try
            {
                await _authService.Revoke(token);
                return Ok("Token revoked successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
