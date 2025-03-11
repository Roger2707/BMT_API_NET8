namespace Store_API.IService
{
    public interface IRabbitMQService
    {
        Task PublishAsync(string queueName, string message);
    }
}
