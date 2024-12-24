using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Store_API.Data;
using Store_API.DTOs;
using Store_API.DTOs.Products;
using Store_API.Helpers;
using Store_API.Models;
using Store_API.Repositories;
using Store_API.Validations;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Store_API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly StoreContext _db;
        private readonly ICSVRepository _csvService;

        public ProductsController(IUnitOfWork unitOfWork, StoreContext db, ICSVRepository csvService)
        {
            _db = db;
            _unitOfWork = unitOfWork;
            _csvService = csvService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            List<Product> products = await _db.Products.Include(p => p.Category).Include(p => p.Brand).ToListAsync();
            if (products == null || products.Count == 0) return NotFound();
            return Ok(products);
        }

        [HttpGet("get-products-page")]
        public async Task<IActionResult> GetProducts([FromQuery] ProductParams productParams)
        {
            List<ProductDTO> products = await _unitOfWork.Product.GetSourceProducts(productParams);
            if(products == null || products.Count == 0) return NotFound();
            Pagination<ProductDTO> productPagination = await _unitOfWork.Product.GetPagination(products, productParams);
            return Ok(productPagination);
        }

        [HttpGet("get-product-detail", Name = "get-product-detail")]
        public async Task<IActionResult> GetProductDetail([FromQuery] int id)
        {
            ProductDTO product = await _unitOfWork.Product.GetById(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] ProductUpsertDTO productDTO)
        {
            if (await _unitOfWork.CheckExisted("Products", productDTO.Name)) 
                return BadRequest(new ProblemDetails { Title = $"Product name {productDTO.Name} is existed !" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int result = 0;
            string error = "";
            try
            {
                result = await _unitOfWork.Product.Create(productDTO);
            }
            catch (Exception ex)
            { 
                error = ex.Message;
            }

            if (result > 0)
            {
                return CreatedAtRoute("get-product-detail", new { id = result });
            }
            return BadRequest(new ProblemDetails { Title = error });
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([Required] int id, [FromForm] ProductUpsertDTO productDTO)
        {
            if (!await _unitOfWork.CheckExisted("Products", id))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string error = "";
            try
            {
                int result = await _unitOfWork.Product.Update(id, productDTO);

                if (result > 0) 
                {
                    var productResponse = await _unitOfWork.Product.GetById(id);
                    return Ok(productResponse);
                }
            }
            catch(Exception ex)
            {
                error = ex.Message;
            }
            return BadRequest(new ProblemDetails { Title = error });
        }

        [HttpPost("change-status")]
        public async Task<IActionResult> ChangeStatusProduct([Required] int id)
        {
            if (!await _unitOfWork.CheckExisted("Products", id))
                return NotFound();

            string error = "";
            int result = 0;
            try
            {
                _unitOfWork.BeginTrans();
                result = await _unitOfWork.Product.ChangeStatus(id);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                error = ex.Message;
                _unitOfWork.Rollback();
            }
            finally
            { 
                _unitOfWork.CloseConnection();
            }
            if(result > 0) return Ok(result);
            return BadRequest(new ProblemDetails { Title = error });
        }

        [HttpPost("import-csv")]
        public async Task<IActionResult> ImportCSV(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
                return BadRequest(new ProblemDetails { Title = "Please attach csv file !"});

            var products = await _csvService.ReadCSV<ProductCSV>(csvFile);
            string error = "";

            try
            {
                _unitOfWork.BeginTrans();

                for (int i = 0; i < products.Count; i++)
                { 
                    await _unitOfWork.Product.InsertCSV(products[i]);
                }

                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                error = ex.Message;
                _unitOfWork.Rollback();
            }
            finally
            {
                _unitOfWork.CloseConnection();
            }

            if (error == "")
                return Ok();
            else
                return BadRequest(new ProblemDetails { Title = error });
        }
    }
}
