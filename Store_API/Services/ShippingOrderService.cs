using Newtonsoft.Json;
using Store_API.DTOs.Orders;
using Store_API.Enums;
using Store_API.IService;
using Store_API.Models.OrderAggregate;
using Store_API.Repositories;
using System.Text;

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
                service_type_id = ServiceShippingType.GHN,
                payment_type_id = PaymentType.PrePaid,
                required_note = "DONOTPPERMITPREVIEW",

                to_name = orderCreateRequest.Username,
                to_phone = orderCreateRequest.PhoneNumber ?? "0776198741",
                to_address = orderCreateRequest.ShippingAdress.StreetAddress,
                to_ward_code = "20308",
                to_district_id = 1444,

                from_name = orderCreateRequest.Username,
                from_phone = "0776193347",
                from_address = "ROGER BMT Test Address",
                from_ward_name = "ROGER BMT Test Ward",
                from_district_name = "ROGER BMT Test Distric Name",
                from_province_name = "ROGER BMT Test Province Name",

                cod_amount = orderCreateRequest.Amount,
                content = shippingContent,

                weight = 1000,
                length = 20,
                width = 15,
                height = 10,

                Items = orderCreateRequest.BasketDTO.Items.Select(item => new
                {
                    name = item.ProductName,
                    quantity = item.Quantity,
                    weight = 1200,
                }).ToList(),
            };

            var ghnToken = _config["GHN:Token"];
            var shopId = _config["GHN:ShopID"];
            var apiUrl = "https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/create";

            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Add("Token", ghnToken);
            _http.DefaultRequestHeaders.Add("ShopId", shopId);

            //
            var jsonData = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await _http.PostAsJsonAsync(apiUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"GHN API error: {response.StatusCode}, {errorContent}");
            }

            var data = await response.Content.ReadFromJsonAsync<dynamic>();
            if (data?.data?.order_code == null)
            {
                throw new InvalidOperationException("Can not get order code from response !");
            }
            string ghnOrderCode = data.data.order_code;

            // Save ShippingOrder
            var shipping = new ShippingOrder
            {
                OrderId = orderCreateRequest.OrderId,
                GHNOrderCode = ghnOrderCode,
                ToName = request.to_name,
                ToPhone = request.to_phone,
                ToAddress = request.to_address,

                ToWard = orderCreateRequest.ShippingAdress.Ward,
                ToDistrict = orderCreateRequest.ShippingAdress.District,
                ToProvince = orderCreateRequest.ShippingAdress.City,

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
