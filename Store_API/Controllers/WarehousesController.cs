using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.Warehouse;
using Store_API.IService;

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
        [Authorize(Policy = "WarehouseAccess")]
        public async Task<IActionResult> GetAll()
        {
            var warehouses = await _warehouseService.GetAll();
            return Ok(warehouses);
        }

        [HttpGet("get-warehouse-detail", Name = "GetDetailWarehouse")]
        [Authorize(Policy = "ViewWarehouseDetails")]
        public async Task<IActionResult> GetById([FromQuery] Guid warehouseId)
        {
            var warehouse = await _warehouseService.GetWarehouseDetail(warehouseId);
            return Ok(warehouse);
        }

        [HttpGet("transactions")]
        [Authorize(Policy = "ViewTransactions")]
        public async Task<IActionResult> GetTransactions([FromQuery] Guid warehouseId)
        {
            // Implementation for getting warehouse transactions
            return Ok();
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
        [Authorize(Policy = "ManageTransactions")]
        public async Task<IActionResult> Update([FromForm] WarehouseUpsertDTO model)
        {
            try
            {
                await _warehouseService.Update(model);
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
                await _warehouseService.Delete(warehouseId);
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
