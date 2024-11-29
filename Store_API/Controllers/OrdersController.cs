using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.Orders;
using Store_API.Models;
using Store_API.Models.Order;
using Store_API.Repositories;

namespace Store_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        public OrdersController(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> GetOrdersUserId(int userId)
        {
            if(userId == 0 || userId.ToString() == "") 
                return BadRequest(new ProblemDetails { Title = "Please enter UserId who want to check !"});

            var order = await _unitOfWork.Order.GetAll(userId);
            if (order == null) return NotFound();

            return Ok(order);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetOrders()
        {
            int userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            var order = await _unitOfWork.Order.GetAll(userId);
            if(order == null) return NotFound();

            return Ok(order);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] ShippingAddressDTO model)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            int userId = user.Id;
            var basket = await _unitOfWork.Basket.GetBasket(User.Identity.Name);

            if(basket == null || basket.Items.Count == 0) return BadRequest(new ProblemDetails { Title = "There are no items in basket!" });

            string error = "";
            try
            {
                _unitOfWork.BeginTrans();

                await _unitOfWork.Order.Create(userId, model, basket);

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

            if(error == "") return Ok();
            else return BadRequest(new ProblemDetails { Title = error });
        }
    }
}
