using Microsoft.AspNetCore.Mvc;
using Store_API.IService;

namespace Store_API.Controllers
{
    [Route("api/technologies")]
    [ApiController]
    public class TechnologiesController : ControllerBase
    {
        private readonly ITechnologyService _technologyService;

        public TechnologiesController(ITechnologyService technologyService)
        {
            _technologyService = technologyService;
        }
        [HttpGet("get-technologies-by-product")]
        public async Task<IActionResult> GetTechnologiesOfProduct(Guid productId)
        {
            var result = await _technologyService.GetTechnologiesOfProduct(productId);
            return Ok(result);
        }
    }
}
