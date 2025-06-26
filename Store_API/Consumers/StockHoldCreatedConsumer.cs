using MassTransit;
using Store_API.Contracts;
using Store_API.Services.IService;

namespace Store_API.Consumers
{
    public class StockHoldCreatedConsumer : IConsumer<StockHoldCreated>
    {
        private readonly IStockHoldService _stockHoldService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<StockHoldCreatedConsumer> _logger;

        public StockHoldCreatedConsumer(IStockHoldService stockHoldService, IPublishEndpoint publishEndpoint, ILogger<StockHoldCreatedConsumer> logger)
        {
            _stockHoldService = stockHoldService;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<StockHoldCreated> context)
        {
            _logger.LogInformation("StockHoldCreatedConsumer: Consuming message for PaymentIntentId: {PaymentIntentId}, UserId: {UserId}",
                context.Message.PaymentIntentId, context.Message.UserId);
            try
            {
                await _stockHoldService.CreateStockHoldAsync(context.Message.PaymentIntentId, context.Message.UserId, context.Message.Items);
                await _publishEndpoint.Publish(
                    new StockHoldExpiredCreated(context.Message.PaymentIntentId)
                    , ctx =>
                    {
                        ctx.Headers.Set("x-delay", 15 * 60 * 1000); // 15 minutes
                        ctx.SetRoutingKey("");
                        ctx.DestinationAddress = new Uri("exchange:my-delayed-exchange");
                    }
                );
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
