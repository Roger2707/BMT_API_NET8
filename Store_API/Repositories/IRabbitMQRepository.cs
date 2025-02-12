using Store_API.DTOs.Orders;
using Store_API.Models.OrderAggregate;

namespace Store_API.Repositories
{
    public interface IRabbitMQRepository
    {
        public Task PublishMessage(Order order);
    }
}
