namespace Store_API.Services.IService
{
    public interface IRabbitMQService
    {
        Task PublishAsync(string queueName, string message);
    }
}
