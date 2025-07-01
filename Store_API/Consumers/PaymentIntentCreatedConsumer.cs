using MassTransit;
using Store_API.Contracts;
using Store_API.Enums;
using Store_API.Services.IService;
using Microsoft.AspNetCore.SignalR;
using Store_API.SignalR;
using Store_API.DTOs.Payments;

namespace Store_API.Consumers
{
    public class PaymentIntentCreatedConsumer : IConsumer<PaymentIntentCreated>
    {
        private readonly ILogger<PaymentIntentCreatedConsumer> _logger;
        private readonly IPaymentService _paymentService;
        private readonly IHubContext<NotificationsHub> _paymentHubContext;

        public PaymentIntentCreatedConsumer(ILogger<PaymentIntentCreatedConsumer> logger, IPaymentService paymentService, IHubContext<NotificationsHub> paymentHubContext)
        {
            _logger = logger;
            _paymentService = paymentService;
            _paymentHubContext = paymentHubContext;
        }

        public async Task Consume(ConsumeContext<PaymentIntentCreated> context)
        {
            int userId = context.Message.UserId;
            Guid requestId = context.Message.RequestId;
            try
            {
                _logger.LogInformation("Processing PaymentIntentCreated for UserId: {UserId}, RequestId: {RequestId}", userId, requestId);

                // Create the payment intent
                var paymentIntent = await _paymentService.CreatePaymentIntentAsync(userId, requestId, context.Message.BasketItems, context.Message.ShippingAddress);

                if (paymentIntent != null)
                {
                    await _paymentHubContext
                        .Clients
                        .Group($"user_{userId}")
                        .SendAsync("PaymentProcessingUpdate", new PaymentProcessingResponse
                        {
                            RequestId = context.Message.RequestId,
                            PaymentIntentId = paymentIntent.Id,
                            ClientSecret = paymentIntent.ClientSecret,
                            Status = PaymentStatus.Success,
                            Success = true
                        });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PaymentIntentCreated for UserId: {UserId}, RequestId: {RequestId}", userId, requestId);          
                // Notify FE about error
                await _paymentHubContext
                    .Clients
                    .Group($"user_{userId}")
                    .SendAsync("PaymentProcessingUpdate", new PaymentProcessingResponse
                    {
                        RequestId = context.Message.RequestId,
                        Status = PaymentStatus.Failed,
                        Success = false,
                        Message = ex.Message
                    });
            }
        }
    }
}
