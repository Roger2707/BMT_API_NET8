using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using Store_API.DTOs.Baskets;
using Store_API.DTOs.Orders;
using Store_API.IService;
using Store_API.Models;
using Store_API.Models.OrderAggregate;
using Store_API.Repositories;
using Store_API.Services;
using Stripe;
using System.Diagnostics;

namespace Store_API.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private IOrderService _orderService;
        private IPaymentService _paymentService;

        private readonly UserManager<User> _userManager;
        private readonly IRabbitMQRepository _rabbitMQService;
        public PaymentsController(IOrderService orderService, IPaymentService paymentService, UserManager<User> userManager, IRabbitMQRepository rabbitMQService)
        {
            _orderService = orderService;
            _paymentService = paymentService;

            _userManager = userManager;
            _rabbitMQService = rabbitMQService;
        }

        

    }
}
