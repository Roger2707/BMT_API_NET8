using Store_API.DTOs.Baskets;
using Store_API.Enums;
using Store_API.Infrastructures;
using Store_API.Models.Inventory;
using Store_API.Services.IService;

namespace Store_API.Services
{
    public class StockHoldService : IStockHoldService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StockHoldService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateStockHoldAsync(string paymentIntentId, int userId, List<BasketItemDTO> basketItems)
        {
            try
            {
                var stockHold = new StockHold
                {
                    PaymentIntentId = paymentIntentId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(1),
                    Status = StockHoldStatus.Holding,
                    Items = basketItems
                                    .Where(item => item.Status == true)
                                    .Select(item => new StockHoldItem
                                    {
                                        ProductDetailId = item.ProductDetailId,
                                        Quantity = item.Quantity
                                    })
                                    .ToList()
                };
                await _unitOfWork.StockHold.AddAsync(stockHold);
            }
            catch(Exception)
            {

            }
        }
    }
}
