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
        public async Task<ActionResult<object>> GetAnalysis(string symbol, [FromQuery] int days = 7)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(symbol))
                {
                    return BadRequest(new { error = "Symbol is required" });
                }

                if (days < 3 || days > 30)
                {
                    return BadRequest(new { error = "Days must be between 3 and 30" });
                }

                var analysis = await _stockService.GetAnalysisAsync(symbol);
                var history = await _stockService.GetHistoryAsync(symbol, days);
                var currentStock = await _stockService.GetStockAsync(symbol);

                var volatility = CalculateVolatility(history);
                var recommendation = GetRecommendation(analysis.TrendPercentage, volatility);

                var detailedAnalysis = new
                {
                    Symbol = analysis.Symbol,
                    CurrentPrice = analysis.CurrentPrice,
                    Analysis = new
                    {
                        MovingAverage = analysis.MovingAverage7Days,
                        Trend = analysis.Trend,
                        TrendPercentage = analysis.TrendPercentage,
                        Volatility = Math.Round(volatility, 2),
                        Recommendation = recommendation
                    },
                    PriceData = new
                    {
                        Current = currentStock?.Price,
                        Change = currentStock?.Change,
                        ChangePercent = currentStock?.ChangePercent,
                        HighLow = new
                        {
                            High = history.Max(),
                            Low = history.Min(),
                            Range = Math.Round(history.Max() - history.Min(), 2)
                        }
                    },
                    TechnicalIndicators = new
                    {
                        RSI = CalculateRSI(history),
                        Support = Math.Round(history.Min() * 1.02m, 2),
                        Resistance = Math.Round(history.Max() * 0.98m, 2)
                    },
                    LastUpdated = analysis.AnalysisDate
                };

                return Ok(detailedAnalysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting analysis for {symbol}");
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }

        [HttpGet("{symbol}/simple")]
        public async Task<ActionResult<StockAnalysis>> GetSimpleAnalysis(string symbol)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(symbol))
                {
                    return BadRequest(new { error = "Symbol is required" });
                }

                var analysis = await _stockService.GetAnalysisAsync(symbol);
                return Ok(analysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting simple analysis for {symbol}");
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }

        private decimal CalculateVolatility(List<decimal> prices)
        {
            if (prices.Count < 2) return 0;

            var returns = new List<decimal>();
            for (int i = 1; i < prices.Count; i++)
            {
                var dailyReturn = (prices[i] - prices[i - 1]) / prices[i - 1];
                returns.Add(dailyReturn);
            }

            var avgReturn = returns.Average();
            var variance = returns.Select(r => (r - avgReturn) * (r - avgReturn)).Average();
            return (decimal)Math.Sqrt((double)variance) * 100;
        }

        private decimal CalculateRSI(List<decimal> prices)
        {
            if (prices.Count < 2) return 50;

            var gains = new List<decimal>();
            var losses = new List<decimal>();

            for (int i = 1; i < prices.Count; i++)
            {
                var change = prices[i] - prices[i - 1];
                if (change > 0)
                {
                    gains.Add(change);
                    losses.Add(0);
                }
                else
                {
                    gains.Add(0);
                    losses.Add(Math.Abs(change));
                }
            }

            var avgGain = gains.Average();
            var avgLoss = losses.Average();

            if (avgLoss == 0) return 100;

            var rs = avgGain / avgLoss;
            var rsi = 100 - (100 / (1 + rs));

            return Math.Round(rsi, 2);
        }

        private string GetRecommendation(decimal trendPercentage, decimal volatility)
        {
            return (trendPercentage, volatility) switch
            {
                (> 5, < 5) => "Strong Buy",
                (> 2, < 8) => "Buy",
                (> 0, < 10) => "Hold",
                (< -2, < 8) => "Sell",
                (< -5, _) => "Strong Sell",
                (_, > 15) => "High Risk - Caution",
                _ => "Hold"
            };
        }
    }
}