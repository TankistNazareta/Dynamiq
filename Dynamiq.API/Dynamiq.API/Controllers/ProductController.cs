using Dynamiq.Application.Commands.Products.Commands;
using Dynamiq.Application.DTOs;
using Dynamiq.Application.Queries.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dynamiq.API.Controllers
{
    [Route("product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public ProductController(
            IMediator mediator,
            ILogger<ProductController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddProductCommand command)
        {
            await _mediator.Send(command);

            _logger.LogInformation("Product was created");

            return Ok("Product was created");
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UpdateProductCommand command)
        {
            await _mediator.Send(command);

            _logger.LogInformation("Updated product with");

            return Ok("Product updated successfully");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _mediator.Send(new GetAllProductsQuery());

            _logger.LogInformation("Retrieved all products, count: {Count}", all?.Count() ?? -1);

            return Ok(all);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var product = await _mediator.Send(new GetProductByIdQuery(id));

            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await _mediator.Send(new DeleteProductCommand(id));

            _logger.LogInformation("Deleted product with ID: {Id}", id);

            return Ok("Product was removed");
        }

        [HttpGet("subscription")]
        public async Task<IActionResult> GetSubscriptions()
        {
            var products = await _mediator.Send(new GetOnlySubscriptionsQuery());

            return Ok(products);
        }

        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetAllProductByCategory(string slug)
        {
            var products = await _mediator.Send(new GetAllProductBySlugQuery(slug));

            return Ok(products);
        }
    }
}
