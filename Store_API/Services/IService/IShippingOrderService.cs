using Store_API.DTOs.Orders;

namespace Store_API.Services.IService
{
    public interface IShippingOrderService
    {
        Task<string> CreateShippingOrder(OrderCreateRequest orderCreateRequest, string shippingContent);
    }
}
