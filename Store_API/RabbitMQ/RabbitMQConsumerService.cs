using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Store_API.DTOs.Payments;
using Store_API.Hubs;
using Store_API.Models.OrderAggregate;
using Store_API.Repositories;
using Store_API.Services;
using System.Text;

namespace Store_API.RabbitMQ
{
    public class RabbitMQConsumerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private IConnection _connection;
        private IModel _channel;
        private readonly Dictionary<string, Func<string, Task>> _handlers;

        public RabbitMQConsumerService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _handlers = new Dictionary<string, Func<string, Task>>
            {
                { "payment_queue", HandlePaymentProcessingAsync },
                { "email_queue", HandleSendEmailAsync }
            };

            ConnectToRabbitMQ();
        }

        private void ConnectToRabbitMQ()
        {
            var factory = new ConnectionFactory { HostName = "localhost", DispatchConsumersAsync = true };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                foreach (var queue in _handlers.Keys)
                {
                    _channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RabbitMQ] Error connecting to RabbitMQ: {ex.Message}");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                foreach (var (queue, handler) in _handlers)
                {
                    var consumer = new AsyncEventingBasicConsumer(_channel);
                    consumer.Received += async (model, ea) =>
                    {
                        var body = Encoding.UTF8.GetString(ea.Body.ToArray());

                        try
                        {
                            await handler(body); // 📌 Gửi tin nhắn đến handler tương ứng
                            _channel.BasicAck(ea.DeliveryTag, false); // ✅ Xác nhận đã xử lý
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[RabbitMQ] Error processing message: {ex.Message}");

                            // ❗ Nếu lỗi, gửi BasicNack để RabbitMQ thử lại sau
                            _channel.BasicNack(ea.DeliveryTag, false, true);
                        }
                    };

                    _channel.BasicConsume(queue, false, consumer); // 👂 Bắt đầu lắng nghe queue
                }

                await Task.Delay(-1, stoppingToken); // 🔥 Đợi vô thời hạn, không cần lặp liên tục
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RabbitMQ] Consumer error: {ex.Message}");
                await Task.Delay(5000, stoppingToken); // 🔥 Nếu lỗi, đợi 5s trước khi retry
            }
        }


        private async Task HandlePaymentProcessingAsync(string message)
        {
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<OrderHub>>();

            var paymentMessage = JsonConvert.DeserializeObject<PaymentProcessingMessage>(message);
            if (paymentMessage == null) return;

            try
            {
                using var transaction = await unitOfWork.BeginTransactionAsync();

                // Tìm thông tin thanh toán dựa vào PaymentIntentId
                var payment = await unitOfWork.Payment.GetByPaymentIntent(paymentMessage.PaymentIntentId);
                if (payment == null)
                {
                    throw new Exception($"Payment not found for IntentId: {paymentMessage.PaymentIntentId}");
                }

                // Tìm đơn hàng liên quan
                var order = await unitOfWork.Order.FirstOrDefaultAsync(payment.OrderId);
                if (order == null)
                {
                    throw new Exception($"Order not found for PaymentIntentId: {paymentMessage.PaymentIntentId}");
                }

                // Cập nhật trạng thái đơn hàng & thanh toán
                payment.Status = OrderStatus.Completed;
                order.Status = OrderStatus.Completed;

                await unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                // Gửi tín hiệu cập nhật trạng thái đơn hàng qua SignalR
                await hubContext.Clients.All.SendAsync("ReceiveOrderUpdate", payment.OrderId, OrderStatus.Completed);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing payment: {ex.Message}");
                throw;
            }
        }

        private async Task HandleSendEmailAsync(string message)
        {
            using var scope = _scopeFactory.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<EmailSenderService>();

            var emailMessage = JsonConvert.DeserializeObject<dynamic>(message);
            if (emailMessage != null)
            {
                await emailService.SendEmailAsync(emailMessage.To, emailMessage.Subject, emailMessage.Body);
            }
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
