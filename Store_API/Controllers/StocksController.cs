using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.Stocks;
using Store_API.IService;

namespace Store_API.Controllers
{
    [Route("api/stocks")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StocksController(IStockService stockService)
        {
            _stockService = stockService;
        }

        #region Retrieve Stock Data

        [HttpGet("get-stock-product-detail")]
        [Authorize(Policy = "ViewStockDetails")]
        public async Task<IActionResult> GetDetailStock([FromQuery] Guid productId)
        {
            var stock = await _stockService.GetStock(productId);
            if (stock == null) return BadRequest(new ProblemDetails { Title = "Stock is not exited !" });
            return Ok(stock);
        }

        #endregion

        #region Retrieve Stock Transaction Data

        [HttpGet("get-stock-transactions")]
        [Authorize(Policy = "ViewStockTransactions")]
        public async Task<IActionResult> GetStockTransaction([FromQuery] Guid productId)
        {
            var stockTransactions = await _stockService.GetStockTransactions(productId);
            if (stockTransactions == null) return BadRequest(new ProblemDetails { Title = "This Product is not have any stock transaction !" });
            return Ok(stockTransactions);
        }

        #endregion

        #region Import / Export Stocks Handlers

        [HttpPost("upsert-stock")]
        [Authorize(Policy = "ManageStock")]
        public async Task<IActionResult> UpsertStock([FromBody] StockUpsertDTO stockUpsertDTO)
        {
            try
            {
                bool result = true;
                if(stockUpsertDTO.TransactionType == 1)
                {
                    result = await _stockService.ImportStock(stockUpsertDTO);
                }
                else if(stockUpsertDTO.TransactionType == 0)
                {
                    result = await _stockService.ExportStock(stockUpsertDTO);
                }
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message});
            }
        }

        #endregion 
    }
}
