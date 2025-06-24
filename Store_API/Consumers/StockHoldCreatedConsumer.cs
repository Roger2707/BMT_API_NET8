using MassTransit;
using Store_API.Contracts;

namespace Store_API.Consumers
{
    public class StockHoldCreatedConsumer : IConsumer<StockHoldCreated>
    {
        public Task Consume(ConsumeContext<StockHoldCreated> context)
        {
            throw new NotImplementedException();
        }
    }
}
