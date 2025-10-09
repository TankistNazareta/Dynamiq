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

        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut]
        public async Task<IActionResult> SetItemQuantity([FromQuery] Guid userId, [FromQuery] Guid productId, [FromQuery] int quantity)
        {
            var cart = await _mediator.Send(new SetQuantityCartItemCommand(userId, productId, quantity));
            return Ok(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromQuery] Guid userId, [FromQuery] Guid productId, [FromQuery] int quantity)
        {
            var cart = await _mediator.Send(new AddCartItemCommand(userId, productId, quantity));
            return Ok(cart);
        }

        [HttpDelete]
        public async Task<IActionResult> Clear([FromQuery] Guid userId)
        {
            await _mediator.Send(new ClearCartCommand(userId));

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetByUserId([FromQuery] Guid userId)
        {
            var cart = await _mediator.Send(new GetCartByUserIdQuery(userId));

            if (cart == null)
                return NotFound();

            return Ok(cart);
        }
    }
}
