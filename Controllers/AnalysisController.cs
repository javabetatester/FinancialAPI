using Microsoft.AspNetCore.Mvc;
using FinancialAPI.Models;
using FinancialAPI.Services;

namespace FinancialAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly IStockService _stockService;
        private readonly ILogger<AnalysisController> _logger;

        public AnalysisController(IStockService stockService, ILogger<AnalysisController> logger)
        {
            _stockService = stockService;
            _logger = logger;
        }

        [HttpGet("{symbol}")]
        public async Task<ActionResult<StockAnalysis>> GetAnalysis(string symbol)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(symbol))
                {
                    return BadRequest("Symbol is required");
                }

                var analysis = await _stockService.GetAnalysisAsync(symbol);
                return Ok(analysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting analysis for {symbol}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}