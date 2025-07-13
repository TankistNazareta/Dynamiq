using Dynamiq.API.Commands.Product;
using Dynamiq.API.Extension.RequestEntity;
using Dynamiq.API.Interfaces;
using Dynamiq.API.Mapping.DTOs;
using Dynamiq.API.Stripe.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dynamiq.API.Controllers
{
    [Route("product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepo _repo;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public ProductController(
            IProductRepo repo,
            IMediator mediator,
            ILogger<ProductController> logger)
        {
            _repo = repo;
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductRequestEntity productRequest)
        {
            var inserted = await _mediator.Send(new CreateProductCommand(productRequest));

            _logger.LogInformation("Created product with ID: {Id}", inserted.Id);

            return CreatedAtAction(nameof(GetById), new { id = inserted.Id }, inserted);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] ProductDto product)
        {
            var updated = await _mediator.Send(new UpdateProductCommand(product));

            _logger.LogInformation("Updated product with ID: {Id}", updated.Id);

            return Ok(updated);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _repo.GetAll();

            _logger.LogInformation("Retrieved all products, count: {Count}", all?.Count() ?? -1);

            return Ok(all);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var product = await _repo.GetById(id);

            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await _mediator.Send(new DeleteProductCommand(id));

            _logger.LogInformation("Deleted product with ID: {Id}", id);

            return Ok("Product was removed");
        }
    }
}
