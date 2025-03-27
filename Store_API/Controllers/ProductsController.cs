using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.Products;
using Store_API.IService;
using System.ComponentModel.DataAnnotations;

namespace Store_API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("get-products-page")]
        public async Task<IActionResult> GetProductsInPage([FromQuery] ProductParams productParams)
        {
            try
            {
                var result = await _productService.GetPageProductDTOs(productParams);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }

        [HttpGet("get-product-detail", Name = "get-product-detail")]
        public async Task<IActionResult> GetProductDetail([FromQuery] Guid id)
        {
            var result = await _productService.GetProductDetail(id);
            if (result == null)
                return BadRequest(new ProblemDetails { Title = "Product not found !" });
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ProductUpsertDTO productDTO)
        {
            if (productDTO.Id == Guid.Empty)
                return BadRequest(new ProblemDetails { Title = "Product Id is Empty" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _productService.CreateProduct(productDTO);

                if (result == Guid.Empty)
                    return BadRequest(new ProblemDetails { Title = "Create Failed !" });

                return Ok(productDTO.Id);
            }
            catch(Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] ProductUpsertDTO productDTO)
        {
            try
            {
                if (productDTO.Id == Guid.Empty)
                    return BadRequest(new ProblemDetails { Title = "Product Id is Empty" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _productService.UpdateProduct(productDTO);

                if (result == Guid.Empty)
                    return BadRequest(new ProblemDetails { Title = "Update Failed !" });

                return Ok(productDTO.Id);
            }
            catch(Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }

        [HttpPost("change-status")]
        public async Task<IActionResult> ChangeStatusProduct([Required] Guid id)
        {
            var result = await _productService.UpdateProductStatus(id);
            if (result == 0)
                return BadRequest(new ProblemDetails { Title = "Change Status Failed" });
            return Ok();
        }
    }
}
