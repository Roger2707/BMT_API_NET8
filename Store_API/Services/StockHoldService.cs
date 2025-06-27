using Store_API.DTOs.Baskets;
using Store_API.DTOs.Stocks;
using Store_API.Enums;
using Store_API.Infrastructures;
using Store_API.Models.Inventory;
using Store_API.Services.IService;

namespace Store_API.Services
{
    public class StockHoldService : IStockHoldService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStockService _stockService;

        public StockHoldService(IUnitOfWork unitOfWork, IStockService stockService)
        {
            _unitOfWork = unitOfWork;
            _stockService = stockService;
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
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15),
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
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task ConfirmStockHoldAsync(string paymentIntentId, int userId)
        {
            try
            {
                var stockHold = await _unitOfWork.StockHold.FindFirstAsync(x => x.PaymentIntentId == paymentIntentId && x.UserId == userId);
                if (stockHold == null)
                    throw new Exception("Stock hold not found");

                stockHold.Status = StockHoldStatus.Confirmed;
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
