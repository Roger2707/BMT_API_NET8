using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store_API.Models.Users;
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
        public async Task<IActionResult> GetProductRate(Guid productId)
        {
            var product = await _unitOfWork.Product.GetProductDTO(productId);
            if (product == null) return NotFound();
            var rating = await _unitOfWork.Rating.GetRating(productId);
            return Ok(rating);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SetRating(Guid productId, double star)
        {
            var product = await _unitOfWork.Product.GetProductDTO(productId);
            if (product == null) return NotFound();

            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            int currentUserId = currentUser.Id;

            string error = "";
            try
            {
                await _unitOfWork.BeginTransactionDapperAsync();
                await _unitOfWork.Rating.SetRating(currentUserId, productId, star);
                await _unitOfWork.CommitAsync();
            }
            catch(Exception ex)
            {
                error = ex.Message;
                await _unitOfWork.RollbackAsync();
            }
            if(error != "") return BadRequest(new ProblemDetails { Title = error });
            return Ok();
        }
    }
}
