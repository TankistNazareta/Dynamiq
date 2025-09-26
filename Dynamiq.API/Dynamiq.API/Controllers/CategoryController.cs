using Dynamiq.Application.Queries.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dynamiq.API.Controllers
{
    [ApiController]
    [Route("category")]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _mediator.Send(new GetAllCategoriesQuery());

            return Ok(categories);
        }

        [HttpGet("child-and-parents")]
        public async Task<IActionResult> GetCategoryWithParentById([FromQuery] Guid id)
        {
            var categoriesSlug = await _mediator.Send(new GetCategoryWithParentByIdQuery(id));

            return Ok(new { CategoriesSlug = categoriesSlug });
        }
    }
}
