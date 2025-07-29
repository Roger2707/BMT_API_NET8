using MassTransit;
using Store_API.Contracts;
using Store_API.DTOs.Stocks;
using Store_API.Enums;
using Store_API.Infrastructures;
using Store_API.Services.IService;
using Stripe;

namespace Store_API.Consumers
{
    public class StockHoldExpiredConsumer : IConsumer<StockHoldExpiredCreated>
    {
        private readonly PaymentIntentService _paymentIntentService;
        private readonly IStockService _stockService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StockHoldExpiredConsumer> _logger;

        public StockHoldExpiredConsumer(PaymentIntentService paymentIntentService, IStockService stockService, IUnitOfWork unitOfWork, ILogger<StockHoldExpiredConsumer> logger)
        {
            _paymentIntentService = paymentIntentService;
            _stockService = stockService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<StockHoldExpiredCreated> context)
        {
            _logger.LogInformation($"StockHoldExpiredConsumer: Processing stock hold expiration for PaymentIntentId {context.Message.PaymentIntentId}");
            try
            {
                var payment = await _unitOfWork.Payment.FindFirstAsync(x => x.PaymentIntentId == context.Message.PaymentIntentId);
                if (payment != null && payment.Status == PaymentStatus.Success) return;

                var stockHold = await _unitOfWork.StockHold.FindFirstAsync(x => x.PaymentIntentId == context.Message.PaymentIntentId, x => x.Items);

                if (stockHold == null || stockHold.Status != StockHoldStatus.Holding)
                    throw new Exception($"[StockHoldExpired] StockHold not found or already confirmed for {context.Message.PaymentIntentId}");

                if (stockHold.Items == null || !stockHold.Items.Any())
                    throw new Exception($"[StockHoldExpired] No items to release for {context.Message.PaymentIntentId}");

                // Change status -> realize that this hold is expired
                stockHold.Status = StockHoldStatus.Released;

                // Release stock back to the warehouse
                foreach (var item in stockHold.Items)
                {
                    var stock = await _unitOfWork.Stock.FindFirstAsync(x => x.ProductDetailId == item.ProductDetailId);
                    if (stock == null)
                        throw new Exception($"Stock for product {item.ProductDetailId} not found");
                    
                    var stockUpsertDTO = new StockUpsertDTO
                    {
                        ProductDetailId = item.ProductDetailId,
                        WarehouseId = stock.WarehouseId,
                        Quantity = item.Quantity,
                    };
                    await _stockService.ImportStock(stockUpsertDTO);
                }
                _logger.LogInformation($"[StockHoldExpired] Released stock successfully for {context.Message.PaymentIntentId}");

                await _paymentIntentService.CancelAsync(context.Message.PaymentIntentId);
                if (payment != null) 
                    payment.Status = PaymentStatus.Failed;

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"[StockHoldExpired] PaymentIntent {context.Message.PaymentIntentId} cancelled and status updated to Failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing stock hold expiration for PaymentIntentId {PaymentIntentId}", context.Message.PaymentIntentId);
                throw;
            }
        }
    }
}
