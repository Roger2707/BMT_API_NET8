using Store_API.Enums;

namespace Store_API.DTOs.Payments
{
    public class PaymentProcessingResponse
    {
        public Guid RequestId { get; set; }
        public bool Success { get; set; }
        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
        public string Message { get; set; }
        public PaymentStatus Status { get; set; }
    }
}
