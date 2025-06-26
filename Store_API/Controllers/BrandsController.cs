using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.Brands;
using Store_API.Services.IService;

namespace Store_API.Controllers
{
    [Route("api/brands")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var brands = await _brandService.GetAll();
            return Ok(brands);
        }

        [HttpGet("get-brand-detail", Name = "GetDetailBrand")]
        public async Task<IActionResult> GetById([FromQuery] Guid id)
        {
            var brand = await _brandService.GetById(id);
            return Ok(brand);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] BrandDTO model)
        {
            try
            {
                await _brandService.Create(model);
                return CreatedAtRoute("GetDetailBrand", new { id = model.Id }, model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] BrandDTO model)
        {
            try
            {
                await _brandService.Update(model);
                return CreatedAtRoute("GetDetailBrand", new { id = model.Id }, model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _brandService.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
