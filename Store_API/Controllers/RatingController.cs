using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store_API.Models;
using Store_API.Repositories;

namespace Store_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        public RatingController(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetProductRate(int productId)
        {
            var product = await _unitOfWork.Product.GetById(productId);
            if (product == null) return NotFound();
            var rating = await _unitOfWork.Rating.GetRating(productId);
            return Ok(rating);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SetRating(int productId, double star)
        {
            var product = await _unitOfWork.Product.GetById(productId);
            if (product == null) return NotFound();

            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            int currentUserId = currentUser.Id;

            string error = "";
            try
            {
                _unitOfWork.BeginTrans();
                await _unitOfWork.Rating.SetRating(currentUserId, productId, star);
                _unitOfWork.Commit();
            }
            catch(Exception ex)
            {
                error = ex.Message;
                _unitOfWork.Rollback();
            }
            finally
            {
                _unitOfWork.CloseConnection();
            }
            if(error != "") return BadRequest(new ProblemDetails { Title = error });
            return Ok();
        }
    }
}
