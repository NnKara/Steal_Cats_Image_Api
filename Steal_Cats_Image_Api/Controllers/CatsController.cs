using Hangfire;
using Microsoft.AspNetCore.Mvc;
using StealCatsImage.Application.Interfaces.ServiceInterfaces;

namespace Steal_Cats_Image_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class CatsController : ControllerBase
    {
        private readonly ICatService _catService;
        private readonly ILogger<CatsController> _logger;

        public CatsController(ICatService catService, ILogger<CatsController> logger)
        {
            _catService = catService;
            _logger = logger;
        }

        [HttpPost("fetch")]
        public IActionResult FetchCats([FromQuery] int limit = 25)
        {          
            if (limit < 25 || limit > 100)
                return BadRequest(new { message = "Limit must be between 25 and 100" });

            try
            {
                var jobId = BackgroundJob.Enqueue<ICatService>(x => x.FetchCatsAsync(limit, CancellationToken.None));

                return Accepted(new
                {
                    message = "Cat fetch job started",
                    jobId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to enqueue cat fetch job (limit={Limit})", limit);
                return StatusCode(503, new { message = "Background job service is temporarily unavailable. Please try again later." });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var cat = await _catService.GetByIdAsync(id, ct);
            if (cat is null)
                return NotFound(new { message = $"Cat with id {id} was not found." });

            return Ok(cat);
        }

        [HttpGet]
        public async Task<IActionResult> GetByTag([FromQuery] string tag,[FromQuery] int page = 1,[FromQuery] int pageSize = 10,CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return BadRequest(new { message = "Tag is required" });

            if (page < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest("Page number must be >= 1 and pageSize between 1 and 100");

                var result = await _catService.GetPagedByTagAsync(tag, page, pageSize, ct);
                return Ok(result);          
        }
    }
}
