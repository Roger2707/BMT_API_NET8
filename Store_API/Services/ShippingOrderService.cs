using Store_API.DTOs.Orders;
using Store_API.Enums;
using Store_API.IService;
using Store_API.Models.OrderAggregate;
using Store_API.Repositories;

namespace Store_API.Services
{
    public class ShippingOrderService : IShippingOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public ShippingOrderService(IUnitOfWork unitOfWork, IUserService userService, IConfiguration config, HttpClient http)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _config = config;
            _http = http;
        }

        public async Task<string> CreateShippingOrder(OrderCreateRequest orderCreateRequest, string shippingContent)
        {
            var request = new
            {
                payment_type_id = PaymentType.PrePaid,
                to_name = orderCreateRequest.Username,
                to_phone = orderCreateRequest.PhoneNumber,
                to_address = orderCreateRequest.ShippingAdress.StreetAddress,
                to_ward_name = orderCreateRequest.ShippingAdress.Ward,
                to_district_name = orderCreateRequest.ShippingAdress.District,
                to_province_name = orderCreateRequest.ShippingAdress.City,
                cod_amount = orderCreateRequest.Amount,
                content = shippingContent,
                weight = 1000,
                length = 20,
                width = 15,
                height = 10,
                service_type_id = ServiceShippingType.GHN
            };

            var ghnToken = _config["GHN:Token"];
            var apiUrl = "https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/create";

            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Add("Token", ghnToken);

            var response = await _http.PostAsJsonAsync(apiUrl, request);
            var data = await response.Content.ReadFromJsonAsync<dynamic>();

            string ghnOrderCode = data?.data?.order_code ?? throw new Exception("GHN Create Order failed");

            // Save ShippingOrder
            var shipping = new ShippingOrder
            {
                OrderId = orderCreateRequest.OrderId,
                GHNOrderCode = ghnOrderCode,
                ToName = request.to_name,
                ToPhone = request.to_phone,
                ToAddress = request.to_address,
                ToWard = request.to_ward_name,
                ToDistrict = request.to_district_name,
                ToProvince = request.to_province_name,
                CODAmount = request.cod_amount,
                Weight = request.weight,
                Length = request.length,
                Width = request.width,
                Height = request.height
            };

            await _unitOfWork.ShippingOrder.AddAsync(shipping);
            return ghnOrderCode;
        }
    }
}
