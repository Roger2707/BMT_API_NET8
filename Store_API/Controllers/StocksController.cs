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

        [HttpGet("get-all-stocks")]
        public async Task<IActionResult> GetAllStocks()
        {
            var stocks = await _stockService.GetStocks();
            return Ok(stocks);
        }

        [HttpGet("get-stock-product-detail", Name = "GetDetailStock")]
        public async Task<IActionResult> GetDetailStock([FromQuery] Guid productId)
        {
            var stock = await _stockService.GetStock(productId);
            if (stock == null) return BadRequest(new ProblemDetails { Title = "Stock is not exited !" });
            return Ok(stock);
        }

        #endregion

        #region Retrieve Stock Transaction Data

        [HttpGet("get-all-stock-transactions")]
        public async Task<IActionResult> GetAllStockTransactions()
        {
            var stockTransactions = await _stockService.GetStockTransactions();
            return Ok(stockTransactions);
        }

        [HttpGet("get-detail-stock-transaction", Name = "GetDetailStockTransaction")]
        public async Task<IActionResult> GetDetailStockTransaction([FromQuery] Guid stockTransactionId)
        {
            var stockTransaction = await _stockService.GetStockTransaction(stockTransactionId);
            if (stockTransaction == null) return BadRequest(new ProblemDetails { Title = "Stock Transactions is not exited !" });
            return Ok(stockTransaction);
        }

        [HttpGet("get-product-detail-stock-transactions", Name = "GetProductDetailStockTransaction")]
        public async Task<IActionResult> GetProductDetailStockTransaction([FromQuery] Guid productId)
        {
            var stockTransactions = await _stockService.GetProductDetailStockTransactions(productId);
            if (stockTransactions == null) return BadRequest(new ProblemDetails { Title = "This Product is not have any stock transaction !" });
            return Ok(stockTransactions);
        }

        #endregion

        #region Import / Export Stocks Handlers

        [HttpGet("get-quantity")]
        public async Task<IActionResult> GetQuantityInStock(Guid productDetailId)
        {
            int quantity = await _stockService.GetCurrentQuantityInStock(productDetailId);
            return Ok(quantity);
        }

        [HttpPost("upsert-stock")]
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
