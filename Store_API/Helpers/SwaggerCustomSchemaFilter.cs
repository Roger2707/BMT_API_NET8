using Azure;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.DataAnnotations;

namespace Store_API.Helpers
{
    public class IgnoreRequiredFieldsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Lặp qua tất cả các tham số trong operation
            foreach (var parameter in operation.Parameters)
            {
                // Kiểm tra xem tham số có phải là từ một model không
                var parameterInfo = context.ApiDescription.ParameterDescriptions.FirstOrDefault(p => p.Name == parameter.Name);
                if (parameterInfo != null && parameterInfo.ModelMetadata != null)
                {
                    // Lấy thông tin về model và đánh dấu tham số không bắt buộc
                    var isRequired = parameterInfo.ModelMetadata.IsRequired;
                    if (isRequired)
                    {
                        parameter.Required = false; // Đặt lại thành false để không yêu cầu
                    }
                }
            }
        }
    }
}
