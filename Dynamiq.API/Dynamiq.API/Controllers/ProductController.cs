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

        public ProductController(IProductRepo repo, IStripeProductService stripeService)
        {
            _stripeProductService = stripeService;
            _repo = repo;
        }

        [HttpPost] 
        public async Task<IActionResult> Post([FromBody] ProductRequestEntity productRequest)
        {
            try
            {
                var productDto = await _stripeProductService.CreateProductStripe(productRequest);
                await _repo.Insert(productDto);

                return Ok(productDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] ProductDto productRequest)
        {
            try
            {
                var productDto = await _stripeProductService.UpdateProductStripe(productRequest);
                await _repo.Update(productDto);

                return Ok(productDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await _repo.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            try
            {
                return Ok(await _repo.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
               var product = await _repo.GetById(id);

                await _repo.Delete(id);
                await _stripeProductService.DeleteProductStripe(product.StripePriceId, product.StripeProductId);

                return Ok("Product was removed");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
