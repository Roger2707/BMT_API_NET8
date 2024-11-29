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
    [Route("api/[controller]/[action]")]
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

        [HttpGet(Name = "GetDetailBasket")]
        [Authorize]
        public async Task<IActionResult> GetBasket()
        {
            var basket = await _unitOfWork.Basket.GetBasket(User.Identity.Name);
            if (basket == null) return NotFound();

            return Ok(basket);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(int productId, int quantity)
        {
            var product = await _unitOfWork.Product.GetById(productId);
            if(product == null) return NotFound();
            string error = "";
            try
            {
                _unitOfWork.BeginTrans();
                await _unitOfWork.Basket.AddItem(User.Identity.Name, productId, quantity);
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

            int maxBasketItemId = await _unitOfWork.GetMaxId("BasketItems");
            var basket = await _unitOfWork.Basket.GetBasket(User.Identity.Name);

            if (error != "") return BadRequest(new ProblemDetails { Title = error });
            return CreatedAtRoute("GetDetailBasket", new { id = maxBasketItemId }, basket);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Remove(int productId, int quantity)
        {
            var product = await _unitOfWork.Product.GetById(productId);
            if (product == null) return NotFound();
            string error = "";
            try
            {
                _unitOfWork.BeginTrans();

                await _unitOfWork.Basket.RemoveItem(User.Identity.Name, productId, quantity);

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

        [HttpPost]
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
