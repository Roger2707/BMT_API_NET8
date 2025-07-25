﻿using Store_API.Enums;

namespace Store_API.DTOs.Orders
{
    public class OrderDapperRow
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public double DeliveryFee { get; set; }
        public string ClientSecret { get; set; }

        // User Address
        public int UserAddressId { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string PostalCode { get; set; }
        public string StreetAddress { get; set; }
        public string Country { get; set; }

        // Order Item
        public int OrderItemId { get; set; }
        public Guid ProductDetailId { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public int Quantity { get; set; }
        public double SubTotal { get; set; }

        // Grand Total
        public double GrandTotal { get; set; }
    }
}
