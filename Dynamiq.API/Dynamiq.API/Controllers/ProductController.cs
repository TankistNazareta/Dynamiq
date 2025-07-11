using Dynamiq.API.Extension.RequestEntity;
using Dynamiq.API.Interfaces;
using Dynamiq.API.Mapping.DTOs;
using Dynamiq.API.Stripe.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dynamiq.API.Controllers
{
    [Route("product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepo _repo;
        private readonly IStripeProductService _stripeProductService;
        private readonly ILogger _logger;

        public ProductController(
            IProductRepo repo,
            IStripeProductService stripeService,
            ILogger<ProductController> logger)
        {
            _repo = repo;
            _stripeProductService = stripeService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductRequestEntity productRequest)
        {
            var productDto = await _stripeProductService.CreateProductStripe(productRequest);
            var inserted = await _repo.Insert(productDto);

            _logger.LogInformation("Created product with ID: {Id}", inserted.Id);

            return CreatedAtAction(nameof(GetById), new { id = inserted.Id }, inserted);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] ProductDto productRequest)
        {
            var updated = await _stripeProductService.UpdateProductStripe(productRequest);
            await _repo.Update(updated);

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
            var product = await _repo.GetById(id);

            await _repo.Delete(id);
            await _stripeProductService.DeleteProductStripe(product.StripePriceId, product.StripeProductId);

            _logger.LogInformation("Deleted product with ID: {Id}", id);

            return Ok("Product was removed");
        }
    }
}
