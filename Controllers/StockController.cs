using Microsoft.AspNetCore.Mvc;
using FinancialAPI.Models;
using FinancialAPI.Services;

namespace FinancialAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StocksController : ControllerBase
    {
        private readonly IStockService _stockService;
        private readonly ILogger<StocksController> _logger;

        public StocksController(IStockService stockService, ILogger<StocksController> logger)
        {
            _stockService = stockService;
            _logger = logger;
        }

        [HttpGet("{symbol}")]
        public async Task<ActionResult<Stock>> GetStock(string symbol)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(symbol))
                {
                    return BadRequest("Symbol is required");
                }

                var stock = await _stockService.GetStockAsync(symbol);

                if (stock == null)
                {
                    return NotFound($"Stock with symbol {symbol} not found");
                }

                return Ok(stock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching stock {symbol}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{symbol}/history")]
        public async Task<ActionResult<List<decimal>>> GetHistory(string symbol, [FromQuery] int days = 7)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(symbol))
                {
                    return BadRequest("Symbol is required");
                }

                if (days < 1 || days > 30)
                {
                    return BadRequest("Days must be between 1 and 30");
                }

                var history = await _stockService.GetHistoryAsync(symbol, days);
                return Ok(new { Symbol = symbol.ToUpper(), Days = days, Prices = history });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching history for {symbol}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}