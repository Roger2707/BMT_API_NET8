using Store_API.Models.OrderAggregate;

namespace Store_API.Repositories
{
    public interface IOrderRepository
    {
        Task Create(Order order);
    }
}
