using Dynamiq.Auth.DTOs;
using Dynamiq.Auth.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dynamiq.Auth.Controllers
{
    [Route("/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ISignUpService _signUpService;
        private readonly ILogInService _logInService;
        private readonly ITokenService _tokenService;

        public AuthController(ISignUpService signUpService, ILogInService logInService, ITokenService tokenService)
        {
            _logInService = logInService;
            _signUpService = signUpService;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromBody] AuthUserDto authUser)
        {
            try
            {
                var token = await _logInService.LogIn(authUser);
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
                await _signUpService.SignUp(authUser);
                return Ok("Please confirm your email, before log in");
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
                var tokens = await _tokenService.Refresh(token);
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
                await _tokenService.Revoke(token);
                return Ok("Token revoked successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
