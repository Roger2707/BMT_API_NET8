using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.Warehouse;
using Store_API.IService;

namespace Store_API.Controllers
{
    [Route("api/warehouses")]
    [ApiController]
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
        public async Task<IActionResult> GetById([FromQuery] Guid id)
        {
            var warehouse = await _warehouseService.GetWarehouseDetail(id);
            return Ok(warehouse);
        }

        [HttpPost("create")]
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
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _warehouseService.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
