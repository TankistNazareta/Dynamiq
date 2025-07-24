using Dynamiq.Application.Commands.Users.Commands;
using Dynamiq.Application.Queries.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var user = await _mediator.Send(new GetUserByIdQuery(id));

            _logger.LogInformation("Retrieved user with id: {Id}", id);

            return Ok(user);
        }

        [HttpPut("changePassword")]
        public async Task<IActionResult> Put([FromBody] ChangeUserPasswordCommand command)
        {
            await _mediator.Send(command);

            _logger.LogInformation("Changed password for user with email: {Email}", command.Email);

            return Ok("You successfully changed your password");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await _mediator.Send(new DeleteUserCommand(id));

            _logger.LogInformation("Deleted user with id: {Id}", id);

            return Ok("DefaultUser was removed");
        }
    }
}
