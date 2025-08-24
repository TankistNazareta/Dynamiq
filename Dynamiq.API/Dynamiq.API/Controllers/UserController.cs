using Dynamiq.Application.Commands.Users.Commands;
using Dynamiq.Application.Queries.Users.Queries;
using Dynamiq.Domain.Enums;
using Dynamiq.Infrastructure.Services.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dynamiq.API.Controllers
{
    [Route("users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public UserController(IMediator mediator, ILogger<UserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromQuery] Guid id)
        {
            var user = await _mediator.Send(new GetUserByIdQuery(id));

            if (user == null)
                return NotFound();

            _logger.LogInformation("Retrieved user with id: {Id}", id);

            return Ok(user);
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> Put([FromBody] ChangeUserPasswordCommand command)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (userEmail != command.Email)
                return Forbid();

            await _mediator.Send(command);

            _logger.LogInformation("Changed password for user with email: {Email}", command.Email);

            return Ok(new { Message = "You successfully changed your password" });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var usersId = User.FindFirst(JwtClaims.UserId)?.Value;
            var usersRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (usersId != id.ToString() && usersRole != RoleEnum.Admin.ToString())
                return Forbid();

            await _mediator.Send(new DeleteUserCommand(id));

            _logger.LogInformation("Deleted user with id: {Id}", id);

            return Ok(new { Message = "user was removed" });
        }
    }
}
