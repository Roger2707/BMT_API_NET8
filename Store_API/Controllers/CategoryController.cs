using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store_API.Models;
using Store_API.Repositories;
using System.ComponentModel.DataAnnotations;

namespace Store_API.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;    
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _unitOfWork.Category.GetAllCategory();
            if(categories == null || categories.Count == 0)
                return NotFound();
            return Ok(categories);
        }

        [HttpGet("get-category-detail")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var category = await _unitOfWork.Category.GetCategoryById(id);
            if (category == null)
                return NotFound();
            return Ok(category);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(string name)
        { 
            if (await _unitOfWork.Category.CheckCategoryExisted(name)) return BadRequest(new ProblemDetails { Title = "Category has existed !"});
            Category category = new() { Name = name };
            await _unitOfWork.Category.Create(category);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtRoute("GetById", new { id = category.Id }, category);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([Required]int id, string name)
        {
            Category category = await _unitOfWork.Category.GetCategoryById(id);
            if (category == null) return NotFound();

            Category updatedCategory = await _unitOfWork.Category.Update(id, name);
            await _unitOfWork.SaveChangesAsync();

            return Ok(updatedCategory);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            Category category = await _unitOfWork.Category.GetCategoryById(id);
            if (category == null) return NotFound();

            await _unitOfWork.Category.Delete(id);
            await _unitOfWork.SaveChangesAsync();

            return Ok();
        }
    }
}
