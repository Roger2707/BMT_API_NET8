using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store_API.Data;
using Store_API.DTOs.Althetes;
using Store_API.DTOs.Products;
using Store_API.Models;
using Store_API.Repositories;
using System.ComponentModel.DataAnnotations;

namespace Store_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AlthetesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly StoreContext _db;
        public AlthetesController(IUnitOfWork unitOfWork, StoreContext db)
        {
            _db = db;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPlayers()
        {
            var althetes = await _unitOfWork.Althete.GetAll();
            if(althetes.Count == 0) return NotFound();
            return Ok(althetes);
        }

        [HttpGet("{id}", Name = "GetAltheteById")]
        public async Task<IActionResult> GetPlayerById(int id)
        {
           var player = await _unitOfWork.Althete.GetPlayerById(id);
           if(player == null) return NotFound();
           return Ok(player);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] AltheteUpsertDTO model)
        {
            int result = 0;
            string error = "";
            try
            {
                await _unitOfWork.Althete.Create(model);
                result = await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (result > 0)
            {
                // fix later 
                return CreatedAtRoute("GetAltheteById", new { id = 1 }, model);
            }
            return BadRequest(new ProblemDetails { Title = error });
        }

        [HttpPut]
        public async Task<IActionResult> Update([Required] int id, [FromForm] AltheteUpsertDTO model)
        {
            if (!await _unitOfWork.CheckExisted("Althetes", id))
                return NotFound();

            string error = "";
            try
            {
                Althete updatedPlayer = await _unitOfWork.Althete.Update(id, model);
                int result = await _unitOfWork.SaveChangesAsync();

                if (result > 0) return Ok(updatedPlayer);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return BadRequest(new ProblemDetails { Title = error });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _unitOfWork.CheckExisted("Althetes", id))
                return NotFound();

            string error = "";
            int result = 0;
            try
            {
                await _unitOfWork.Althete.Delete(id);
                result = await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            if (error != "") return Ok(result);
            return BadRequest(new ProblemDetails { Title = error });
        }
    }
}
