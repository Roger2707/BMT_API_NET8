using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.Promotions;
using Store_API.Models;
using Store_API.Repositories;

namespace Store_API.Controllers
{
    [Route("api/promotions")]
    [ApiController]
    public class PromotionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public PromotionsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(string? start, string? end)
        {
            var promotions = await _unitOfWork.Promotion.GetAll(start, end);
            if (promotions == null || promotions.Count <= 0) 
                return BadRequest(new ProblemDetails { Title = "No Promotions" });

            return Ok(promotions);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] PromotionUpsertDTO promotion)
        {
            string error = "";
            try
            {
                _unitOfWork.BeginTrans();

                await _unitOfWork.Promotion.Create(promotion);

                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                error = ex.Message;
                _unitOfWork.Rollback();
            }
            finally
            {
                _unitOfWork.CloseConnection();
            }
            if (error != "") return BadRequest(new ProblemDetails { Title = error });
            return Ok();
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(int id, [FromForm] PromotionUpsertDTO promotion)
        {
            string error = "";
            try
            {
                _unitOfWork.BeginTrans();

                await _unitOfWork.Promotion.Update(id, promotion);

                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                error = ex.Message;
                _unitOfWork.Rollback();
            }
            finally
            {
                _unitOfWork.CloseConnection();
            }
            if (error != "") return BadRequest(new ProblemDetails { Title = error });
            return Ok();
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            string error = "";
            try
            {
                _unitOfWork.BeginTrans();

                await _unitOfWork.Promotion.Delete(id);

                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                error = ex.Message;
                _unitOfWork.Rollback();
            }
            finally
            {
                _unitOfWork.CloseConnection();
            }
            if (error != "") return BadRequest(new ProblemDetails { Title = error });
            return Ok();
        }
    }
}
