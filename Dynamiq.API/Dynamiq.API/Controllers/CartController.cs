using Dynamiq.Application.Commands.Carts.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dynamiq.API.Controllers
{
    [ApiController]
    [Route("cart")]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> AddItemToCart([FromRoute] Guid userId, Guid productId, int quantity)
        {
            var cart = await _mediator.Send(new AddItemToCartCommand(userId, productId, quantity));

            return Ok(cart);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> RemoveItem([FromRoute] Guid userId, Guid productId, int quantity)
        {
            var cart = await _mediator.Send(new RemoveItemFromCartCommand(userId, productId, quantity));

            return Ok(cart);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Clear([FromRoute] Guid userId)
        {
            await _mediator.Send(new ClearCartCommand(userId));

            return Ok();
        }
    }
}
