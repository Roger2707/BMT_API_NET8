using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Store_API.Hubs;
using System.Text;
using System;
using Store_API.Data;
using Store_API.Models.OrderAggregate;

namespace Store_API.RabbitMQ
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _config;

        public RabbitMQConsumer(IServiceProvider serviceProvider, IConfiguration config)
        {
            _serviceProvider = serviceProvider;
            _config = config;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_config["RabbitMQ:HostName"]),
                UserName = _config["RabbitMQ:UserName"],
                Password = _config["RabbitMQ:Password"]
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: _config["RabbitMQ:QueueName"], durable: true, exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<StoreContext>();

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var order = JsonConvert.DeserializeObject<Order>(message);

                order.Status = OrderStatus.Pending;
                await context.SaveChangesAsync();

                // Giả lập thanh toán thành công
                await Task.Delay(5000); 

                order.Status = OrderStatus.Completed;
                await context.SaveChangesAsync();

                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<OrderHub>>();
                await hubContext.Clients.All.SendAsync("OrderStatusUpdated", order.Id, order.Status);
            };

            channel.BasicConsume(queue: _config["RabbitMQ:QueueName"], autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }
    }
}
