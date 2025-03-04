using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store_API.Data;
using Store_API.DTOs;
using Store_API.Models;
using Store_API.Repositories;
using System.ComponentModel.DataAnnotations;

namespace Store_API.Controllers
{
    [Route("api/brands")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly StoreContext _db;
        private readonly IUnitOfWork _unitOfWork;

        public BrandsController(StoreContext db, IUnitOfWork unitOfWork)
        {
            _db = db;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        { 
            List<Brand> brands = await _unitOfWork.Brand.GetAllBrands();
            if(brands == null || brands.Count == 0) return NotFound();
            return Ok(brands);
        }

        [HttpGet("get-brand-detail")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        { 
            var brand = await _unitOfWork.Brand.GetBrandById(id);
            if(brand == null) return NotFound();
            return Ok(brand);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] BrandDTO brandDTO)
        {
            if (await _unitOfWork.Brand.CheckExisted(brandDTO.Name)) 
                return BadRequest(new ProblemDetails { Title = $"Brand with name {brandDTO.Name} is exited !" });
            var brand = new Brand() { Name = brandDTO.Name, Country = brandDTO.Country };

            await _unitOfWork.Brand.Create(brand);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtRoute("GetBrandById", new { id = brand.Id }, brand);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([Required] int id, [FromForm] BrandDTO brandDTO)
        {
            if (!await _unitOfWork.Brand.CheckExisted(id))
                return NotFound();

            Brand updatedBrand = await _unitOfWork.Brand.Update(id, brandDTO);
            await _unitOfWork.SaveChangesAsync();
            return Ok(updatedBrand);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            Brand brand = await _unitOfWork.Brand.GetBrandById(id);
            if (brand == null) return NotFound();

            await _unitOfWork.Brand.Delete(id);
            await _unitOfWork.SaveChangesAsync();

            return Ok();
        }
    }
}
