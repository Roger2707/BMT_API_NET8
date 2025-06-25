using MassTransit;
using Store_API.Contracts;
using Store_API.IService;

namespace Store_API.Consumers
{
    public class StockHoldCreatedConsumer : IConsumer<StockHoldCreated>
    {
        private readonly IStockHoldService _stockHoldService;
        private readonly IPublishEndpoint _publishEndpoint;

        public StockHoldCreatedConsumer(IStockHoldService stockHoldService, IPublishEndpoint publishEndpoint)
        {
            _stockHoldService = stockHoldService;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<StockHoldCreated> context)
        {
            try
            {
                await _stockHoldService.CreateStockHoldAsync(context.Message.PaymentIntentId, context.Message.UserId, context.Message.Items);
                await _publishEndpoint.Publish<StockHoldExpiredCreated>(new
                {
                    PaymentIntentId = context.Message.PaymentIntentId
                }, ctx =>
                {
                    ctx.Headers.Set("x-delay", 15 * 60 * 1000); // 15 minutes
                    ctx.SetRoutingKey(""); // hoặc routing key tương ứng
                    ctx.DestinationAddress = new Uri("exchange:my-delayed-exchange");
                });

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
