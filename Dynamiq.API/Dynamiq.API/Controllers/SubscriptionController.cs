using Dynamiq.Application.Commands.Subscriptions.Command;
using Dynamiq.Application.Commands.Subscriptions.Commands;
using Dynamiq.Application.DTOs.AccountDTOs;
using Dynamiq.Application.Queries.Subscriptions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dynamiq.API.Controllers
{
    [Route("subscription")]
    [ApiController]
    [Authorize]
    public class SubscriptionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SubscriptionController(IMediator mediator)
        {
            _mediator = mediator;   
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var res = await _mediator.Send(new GetAllSubscriptionsQuery());
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddSubscriptionCommand command)
        {
            await _mediator.Send(command);

            return Ok("Subscription was created");
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UpdateSubscriptionCommand command)
        {
            await _mediator.Send(command);

            return Ok("Subscription was updated");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] Guid id)
        {
            await _mediator.Send(new DeleteSubscriptionCommand(id));

            return Ok("Subscription was removed");
        }
    }
}
