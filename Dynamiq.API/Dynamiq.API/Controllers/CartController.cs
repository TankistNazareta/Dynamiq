using Dynamiq.Application.Commands.Carts.Commands;
using Dynamiq.Application.Queries.Carts.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
        public async Task<IActionResult> AddItemToCart([FromQuery] Guid userId, [FromQuery] Guid productId, [FromQuery] int quantity)
        {
            var cart = await _mediator.Send(new AddItemToCartCommand(userId, productId, quantity));

            return Ok(cart);
        }

        [HttpPut]
        public async Task<IActionResult> RemoveItem([FromQuery] Guid userId, [FromQuery] Guid productId, [FromQuery] int quantity)
        {
            var cart = await _mediator.Send(new RemoveItemFromCartCommand(userId, productId, quantity));

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
