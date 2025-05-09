﻿using Store_API.DTOs.Baskets;
using Store_API.DTOs.Orders;
using Stripe;

namespace Store_API.Repositories
{
    public interface IPaymentService
    {
        Task<PaymentIntent> CreatePaymentIntentAsync(BasketDTO basket, ShippingAddressDTO shippingAddress);
        Task HandleStripeWebhookAsync(Event stripeEvent);
    }
}
