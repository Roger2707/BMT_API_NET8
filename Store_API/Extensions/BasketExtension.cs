﻿using Store_API.DTOs.Baskets;

namespace Store_API.Extensions
{
    public static class BasketExtension
    {
        public static BasketDTO MapBasket(this List<BasketDapperRow> result)
        {
            List<BasketItemDTO> items = new List<BasketItemDTO>();
            foreach (var item in result)
            {
                var itemDTO = new BasketItemDTO()
                {
                    BasketItemId = item.BasketItemId,
                    ProductDetailId = item.ProductDetailId,
                    ProductName = item.ProductName,
                    ProductFirstImage = item.ProductFirstImage,
                    Quantity = item.Quantity,
                    OriginPrice = item.OriginPrice,
                    DiscountPercent = item.DiscountPercent,
                    DiscountPrice = item.DiscountPrice,
                    Status = item.Status,
                };

                items.Add(itemDTO);
            }

            double totalPrice = items.Where(i => i.Status == true).Sum(i => i.DiscountPrice * i.Quantity);

            BasketDTO basket = new BasketDTO
            {
                Id = result[0].Id,
                UserId = result[0].UserId,
                Items = items,
                GrandTotal = totalPrice,
            };

            return basket;
        }
    }
}
