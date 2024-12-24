using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.Baskets;
using Store_API.Models;
using Store_API.Repositories;

namespace Store_API.Controllers
{
    [Route("api/baskets")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        public BasketController(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> GetCustomerBasket(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return NotFound();

            var basket = await _unitOfWork.Basket.GetBasket(username);
            if (basket == null) return NotFound();

            return Ok(basket);
        }

        [HttpGet("get-detail-basket")]
        [Authorize]
        public async Task<IActionResult> GetDetailBasket()
        {
            var basket = await _unitOfWork.Basket.GetBasket(User.Identity.Name);
            if (basket == null) return NotFound();

            return Ok(basket);
        }

        [Authorize]
        [HttpPost("upsert-basket")]
        public async Task<IActionResult> UpsertBasket(int productId)
        {
            var product = await _unitOfWork.Product.GetById(productId);
            if (product == null) return BadRequest(new ProblemDetails { Title = $"Product Id: {productId} not found !" });

            string error = "";
            try
            {
                int userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
                await _unitOfWork.Basket.HandleBasketMode(userId, productId, true);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            finally
            {
                _unitOfWork.CloseConnection();
            }

            if (error != "") return BadRequest(new ProblemDetails { Title = error });

            var basket = await _unitOfWork.Basket.GetBasket(User.Identity.Name);
            return CreatedAtRoute("get-detail-basket", basket);
        }

        [Authorize]
        [HttpPost("toggle-status-item")]
        public async Task<IActionResult> ToggleItemsStatus(int itemId)
        {
            string error = "";
            try
            {
                _unitOfWork.BeginTrans();

                await _unitOfWork.Basket.ToggleStatusItems(User.Identity.Name, itemId);

                _unitOfWork.Commit();
            }
            catch(Exception ex)
            {
                error = ex.Message;
                _unitOfWork.Rollback();
            }
            finally {  _unitOfWork.CloseConnection();}

            if(error != "") return BadRequest(new ProblemDetails { Title = error });

            var basket = await _unitOfWork.Basket.GetBasket(User.Identity.Name);
            return CreatedAtRoute("GetDetailBasket", basket);
        }
    }
}
