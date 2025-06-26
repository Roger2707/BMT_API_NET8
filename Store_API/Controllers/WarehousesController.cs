using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.Warehouse;
using Store_API.Helpers;
using Store_API.Services.IService;
using System.Security.Claims;

namespace Store_API.Controllers
{
    [Route("api/warehouses")]
    [ApiController]
    [Authorize]
    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehousesController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var warehouses = await _warehouseService.GetAll();
            return Ok(warehouses);
        }

        [HttpGet("get-warehouse-detail", Name = "GetDetailWarehouse")]
        [Authorize(Policy = "WarehouseAccess")]
        public async Task<IActionResult> GetById([FromQuery] Guid warehouseId)
        {
            var warehouse = await _warehouseService.GetWarehouseDetail(warehouseId);
            return Ok(warehouse);
        }

        [HttpPost("create")]
        [Authorize(Policy = "ManageWarehouses")]
        public async Task<IActionResult> Create([FromForm] WarehouseUpsertDTO model)
        {
            try
            {
                await _warehouseService.Create(model);
                return CreatedAtRoute("GetDetailWarehouse", new { id = model.Id }, model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        [Authorize(Policy = "ManageWarehouses")]
        public async Task<IActionResult> Update([FromForm] WarehouseUpsertDTO model)
        {
            try
            {
                var userId = CF.GetInt(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                await _warehouseService.Update(model, userId);
                return CreatedAtRoute("GetDetailWarehouse", new { id = model.Id }, model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete")]
        [Authorize(Policy = "ManageWarehouses")]
        public async Task<IActionResult> Delete([FromQuery] Guid warehouseId)
        {
            try
            {
                var userId = CF.GetInt(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                await _warehouseService.Delete(warehouseId, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-product-quantity-in-warehouse")]
        public async Task<IActionResult> GetWarehouseProductQuantity(Guid productDetailId)
        {
            try
            {
                if(productDetailId != Guid.Empty)
                {
                    var result = await _warehouseService.GetProductQuantityInWarehouse(productDetailId);
                    return Ok(result);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
