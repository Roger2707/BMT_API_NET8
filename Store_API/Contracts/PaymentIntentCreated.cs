using Store_API.DTOs.Baskets;
using Store_API.DTOs.Orders;

namespace Store_API.Contracts
{
    public record PaymentIntentCreated(Guid RequestId, int UserId, List<BasketItemDTO> BasketItems, ShippingAddressDTO ShippingAddress);
}
