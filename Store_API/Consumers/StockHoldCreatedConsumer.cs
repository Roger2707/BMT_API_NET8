using MassTransit;
using Store_API.Contracts;
using Store_API.Infrastructures;
using Store_API.Services.IService;

namespace Store_API.Consumers
{
    public class StockHoldCreatedConsumer : IConsumer<StockHoldCreated>
    {
        private readonly ILogger<StockHoldCreatedConsumer> _logger;
        private readonly IStockHoldService _stockHoldService;
        private readonly IUnitOfWork _unitOfWork;

        public StockHoldCreatedConsumer(IStockHoldService stockHoldService, IUnitOfWork unitOfWork, ILogger<StockHoldCreatedConsumer> logger)
        {
            _stockHoldService = stockHoldService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<StockHoldCreated> context)
        {
            _logger.LogInformation("StockHoldCreatedConsumer: Consuming message for PaymentIntentId: {PaymentIntentId}, UserId: {UserId}",
                context.Message.PaymentIntentId, context.Message.UserId);
            try
            {
                await _stockHoldService.CreateStockHoldAsync(context.Message.PaymentIntentId, context.Message.UserId, context.Message.Items);
                await context.Publish(
                    new StockHoldExpiredCreated(context.Message.PaymentIntentId)
                    , ctx =>
                    {
                        ctx.Headers.Set("x-delay", 30000);
                    }
                );
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("StockHoldCreatedConsumer: Stock hold created and expiration message published for PaymentIntentId: {PaymentIntentId}", context.Message.PaymentIntentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "StockHoldCreatedConsumer: Error processing StockHoldCreated for PaymentIntentId: {PaymentIntentId}, UserId: {UserId}",
                    context.Message.PaymentIntentId, context.Message.UserId);
                throw;
            }
        }
    }
}
