using Newtonsoft.Json;
using RabbitMQ.Client;
using Store_API.DTOs.Orders;
using Store_API.Models.OrderAggregate;
using Store_API.Repositories;
using System.Text;

namespace Store_API.Services
{
    public class RabbitMQService : IRabbitMQRepository
    {
        private readonly IConfiguration _config;
        public RabbitMQService(IConfiguration config)
        {
            _config = config;
        }
        public async Task PublishMessage(Order order)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_config["RabbitMQ:HostName"]),
                UserName = _config["RabbitMQ:UserName"],
                Password = _config["RabbitMQ:Password"]
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: _config["RabbitMQ:QueueName"], durable: true, exclusive: false, autoDelete: false);

            var message = JsonConvert.SerializeObject(order);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: _config["RabbitMQ:QueueName"], basicProperties: null, body: body);
        }
    }
}
