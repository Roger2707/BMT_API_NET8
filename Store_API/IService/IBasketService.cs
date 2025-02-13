using Store_API.DTOs;
using Store_API.DTOs.Baskets;

namespace Store_API.IService
{
    public interface IBasketService
    {
        Task<BasketDTO> GetBasket(string userName);
        Task<BasketDTO> HandleBasketMode(int userId, int productId, int mode);
        Task<Result<BasketDTO>> UpdateBasketPayment(string paymentIntentId, string clientSecret, string username);
    }
}
