using Store_API.DTOs.Orders;

namespace Store_API.IService
{
    public interface IShippingOrderService
    {
        Task<string> CreateShippingOrder(OrderCreateRequest orderCreateRequest, string shippingContent);
    }
}
