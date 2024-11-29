using Microsoft.AspNetCore.Mvc.ModelBinding;
using Store_API.DTOs.Products;
using Store_API.Helpers;

namespace Store_API.Validations
{
    public class ProductUpsertModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var name = bindingContext.ValueProvider.GetValue("Name").FirstValue;
            var price = bindingContext.ValueProvider.GetValue("Price").FirstValue;
            var description = bindingContext.ValueProvider.GetValue("Description").FirstValue;
            var quantity = bindingContext.ValueProvider.GetValue("QuantityInStock").FirstValue;
            var category = bindingContext.ValueProvider.GetValue("CategoryId").FirstValue;
            var brand = bindingContext.ValueProvider.GetValue("BrandId").FirstValue;
            var imageUrl = bindingContext.ValueProvider.GetValue("ImageUrl").FirstValue;
            var status = bindingContext.ValueProvider.GetValue("ProductStatus").FirstValue;
            var created = bindingContext.ValueProvider.GetValue("Created").FirstValue;

            var model = new ProductUpsertDTO
            {
                Name = name,
                Price = CF.GetDouble(price),
                Description = description,
                QuantityInStock = CF.GetInt(quantity),
                ProductStatus = CF.GetInt(status) == 1 ? Models.ProductStatus.Active : Models.ProductStatus.InActive,
                BrandId = CF.GetInt(brand),
                CategoryId = CF.GetInt(category),
            };

            //if(string.IsNullOrEmpty(model.Name))
            //{
            //    bindingContext.ModelState.AddModelError("name", "Name is required ! Input It");
            //}

            // Nếu model state có lỗi, bạn vẫn có thể trả về model
            bindingContext.Result = ModelBindingResult.Success(model); // Hoặc Failed() nếu bạn muốn bỏ qua

            return Task.CompletedTask;
        }
    }
}
