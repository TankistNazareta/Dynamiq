using Dynamiq.API.Interfaces;
using Dynamiq.Application.Commands.Carts.Commands;
using Dynamiq.Application.Queries.Carts.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dynamiq.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("cart")]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserContextService _userContext;

        public CartController(IMediator mediator, IUserContextService userContext)
        {
            _mediator = mediator;
            _userContext = userContext;
        }

        [HttpPut]
        public async Task<IActionResult> SetItemQuantity([FromQuery] Guid productId, [FromQuery] int quantity)
        {
            var command = new SetQuantityCartItemCommand(productId, quantity)
            {
                UserId = _userContext.GetUserId()
            };

            var cart = await _mediator.Send(command);
            return Ok(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromQuery] Guid productId, [FromQuery] int quantity)
        {
            var command = new AddCartItemCommand(productId, quantity)
            {
                UserId = _userContext.GetUserId()
            };

            var cart = await _mediator.Send(command);
            return Ok(cart);
        }

        [HttpDelete]
        public async Task<IActionResult> Clear()
        {
            var command = new ClearCartCommand()
            {
                UserId = _userContext.GetUserId()
            };

            await _mediator.Send(command);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetByUserId()
        {
            var userId = _userContext.GetUserId();
            var cart = await _mediator.Send(new GetCartByUserIdQuery(userId));

            if (cart == null)
                return NotFound();

            return Ok(cart);
        }
    }

}
