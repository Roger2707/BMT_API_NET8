using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Store_API.DTOs.Products;
using Store_API.IService;
using Store_API.Repositories;
using System.ComponentModel.DataAnnotations;

namespace Store_API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(IProductService productService, IUnitOfWork unitOfWork)
        {
            _productService = productService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("get-products-page")]
        public async Task<IActionResult> GetProducts([FromQuery] ProductParams productParams)
        {
            List<ProductDTO> products = await _productService.GetSourceProducts(productParams);
            if(products == null || products.Count == 0) return NotFound();
            var result = await _productService.GetPagination(products, productParams);

            if(!result.IsSuccess) return BadRequest(new ProblemDetails { Title = result.Errors[0] });
            return Ok(result.Data);
        }

        [HttpGet("get-product-detail", Name = "get-product-detail")]
        public async Task<IActionResult> GetProductDetail([FromQuery] Guid id)
        {
            var result = await _unitOfWork.Product.GetById(id);

            if (!result.IsSuccess)
                return BadRequest(new ProblemDetails { Title = result.Errors[0] });

            return Ok(result.Data);
        }

        [HttpGet("get-technologies")]
        public async Task<IActionResult> GetTechnologies(int productId)
        {
            if (string.IsNullOrEmpty(productId.ToString())) return BadRequest(new ProblemDetails { Title = "Product Id is Empty" });
            try
            {
                var teches = await _productService.GetTechnologies(productId);
                return Ok(teches);
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] ProductUpsertDTO productDTO)
        {
            if (await _unitOfWork.CheckExisted("Products", productDTO.Name)) 
                return BadRequest(new ProblemDetails { Title = $"Product name {productDTO.Name} is existed !" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _unitOfWork.Product.Create(productDTO);

            if(!result.IsSuccess)
                return BadRequest(new ProblemDetails { Title = result.Errors[0] });

            return CreatedAtRoute("get-product-detail", new { id = result.Data }); // productId
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] ProductUpsertDTO productDTO)
        {
            if (productDTO.Id == null || productDTO.Id == Guid.Empty)
                return BadRequest(new ProblemDetails { Title = "Product Id is Empty" });

            var product = await _unitOfWork.Product.GetById(productDTO.Id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _unitOfWork.Product.Update(productDTO);

            if (!result.IsSuccess)
                return BadRequest(new ProblemDetails { Title = result.Errors[0] });

            return Ok();
        }

        [HttpPost("change-status")]
        public async Task<IActionResult> ChangeStatusProduct([Required] int id)
        {
            if (!await _unitOfWork.CheckExisted("Products", id))
                return NotFound();

            var result = await _unitOfWork.Product.ChangeStatus(id);

            if (!result.IsSuccess)
                return BadRequest(new ProblemDetails { Title = result.Errors[0] });

            return Ok();
        }

        [HttpPost("import-csv")]
        public async Task<IActionResult> ImportCSV(IFormFile csvFile)
        {
            var result = await _unitOfWork.Product.InsertCSV(csvFile);

            if (!result.IsSuccess)
                return BadRequest(new ProblemDetails { Title = result.Errors[0] });

            return Ok();
        }
    }
}
