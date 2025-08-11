using Dynamiq.Application.Commands.Carts.Commands;
using Dynamiq.Application.Queries.Carts.Queries;
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

        [HttpPost]
        public async Task<IActionResult> AddItemToCart(AddItemToCartCommand command)
        {
            var cart = await _mediator.Send(command);

            return Ok(cart);
        }

        [HttpPut]
        public async Task<IActionResult> RemoveItem(RemoveItemFromCartCommand command)
        {
            var cart = await _mediator.Send(command);

            return Ok(cart);
        }

        [HttpDelete]
        public async Task<IActionResult> Clear(ClearCartCommand command)
        {
            await _mediator.Send(command);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetByUserId([FromBody] Guid userId)
        {
            var cart = await _mediator.Send(new GetCartByUserIdQuery(userId));

            if (cart == null)
                return NotFound();

            return Ok(cart);
        }
    }
}
