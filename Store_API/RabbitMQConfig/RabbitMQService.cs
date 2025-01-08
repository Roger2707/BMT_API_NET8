using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Store_API.RabbitMQConfig
{
    public class RabbitMQService
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly string _queueName;
        private readonly MessageQueue _messageQueue;

        public RabbitMQService(IConfiguration configuration, MessageQueue messageQueue)
        {
            var rabbitConfig = configuration.GetSection("RabbitMQ");
            _queueName = rabbitConfig["QueueName"];
            _messageQueue = messageQueue;

            _connectionFactory = new ConnectionFactory
            {
                HostName = rabbitConfig["HostName"],
                UserName = rabbitConfig["UserName"],
                Password = rabbitConfig["Password"],
                DispatchConsumersAsync = true
            };

            StartConsuming();
        }

        public async Task SendMessageAsync<T>(T message)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            await Task.Run(() =>
            {
                channel.BasicPublish(exchange: "",
                                     routingKey: _queueName,
                                     basicProperties: properties,
                                     body: messageBody);
            });
        }

        private void StartConsuming()
        {
            var connection = _connectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (sender, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _messageQueue.Enqueue(message);

                channel.BasicAck(eventArgs.DeliveryTag, false);
                await Task.CompletedTask;
            };

            channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
        }
    }
}
