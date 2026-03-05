using Microsoft.AspNetCore.Mvc;
using StealCatsImage.Application.Interfaces.ServiceInterfaces;

namespace Steal_Cats_Image_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class CatsController : ControllerBase
    {
        private readonly ICatService _catService;

        public CatsController(ICatService catService)
        {
            _catService = catService;
        }

        [HttpPost("fetch")]
        public async Task<IActionResult> FetchCats(CancellationToken ct)
        {
            var inserted = await _catService.FetchCatsAsync(25, ct);

            return Ok(new
            {
                message = "Cats fetched successfully",
                inserted
            });
        }

        //Retrieve cat image by id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var cat = await _catService.GetByIdAsync(id, ct);
            if (cat is null)
                return NotFound(new { message = $"Cat with id {id} was not found." });

            return Ok(cat);
        }

        //Retrieve cat images with paging
        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var result = await _catService.GetPagedAsync(page, pageSize, ct);

            return Ok(new
            {
                page,
                pageSize,
                totalCount = result.TotalCount,
                items = result.Items
            });
        }

        //Retrieve cat images filtered by tag with paging
        [HttpGet("by-tag")]
        public async Task<IActionResult> GetByTag(
            [FromQuery] string tag,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return BadRequest(new { message = "Query parameter 'tag' is required." });

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var result = await _catService.GetPagedByTagAsync(tag, page, pageSize, ct);

            return Ok(new
            {
                tag,
                page,
                pageSize,
                totalCount = result.TotalCount,
                items = result.Items
            });
        }
    }
}
