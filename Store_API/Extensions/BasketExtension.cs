using Store_API.DTOs.Baskets;
using Store_API.Helpers;

namespace Store_API.Extensions
{
    public static class BasketExtension
    {
        public static BasketDTO MapBasket(this List<dynamic> result)
        {
            List<BasketItemDTO> items = new List<BasketItemDTO>();

            // Map Basket to BasketDTO
            foreach (var item in result)
            {

                BasketItemDTO itemDTO = new BasketItemDTO()
                {
                    BasketItemId = item.BasketItemId,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductFirstImage = item.ProductFirstImage,
                    Quantity = 0,
                    OriginPrice = 0,
                    DiscountPercent = 0,
                    DiscountPrice = 0,
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
