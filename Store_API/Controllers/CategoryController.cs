using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.Categories;
using Store_API.IService;

namespace Store_API.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAll();
            return Ok(categories);
        }

        [HttpGet("get-category-detail", Name = "GetDetailCategory")]
        public async Task<IActionResult> GetById([FromQuery] Guid id)
        {
            var category = await _categoryService.GetById(id);
            return Ok(category);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CategoryDTO model)
        {
            try
            {
                await _categoryService.Create(model);
                return CreatedAtRoute("GetDetailCategory", new { id = model.Id }, model);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }         
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] CategoryDTO model)
        {
            try
            {
                await _categoryService.Update(model);
                return CreatedAtRoute("GetDetailCategory", new { id = model.Id }, model);
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
                await _categoryService.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
