using FinancialAPI.Models;
using Microsoft.Extensions.Caching.Memory;

namespace FinancialAPI.Services
{
    public class StockService : IStockService
    {
        private readonly YahooFinanceService _yahooService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<StockService> _logger;

        public StockService(YahooFinanceService yahooService, IMemoryCache cache, ILogger<StockService> logger)
        {
            _yahooService = yahooService;
            _cache = cache;
            _logger = logger;
        }

        public async Task<Stock?> GetStockAsync(string symbol)
        {
            var cacheKey = $"stock_{symbol.ToUpper()}";

            if (_cache.TryGetValue(cacheKey, out Stock? cachedStock))
            {
                _logger.LogInformation($"Cache hit for {symbol}");
                return cachedStock;
            }

            _logger.LogInformation($"Cache miss for {symbol}, fetching from external API");
            var stock = await _yahooService.GetStockAsync(symbol);

            if (stock != null)
            {
                _cache.Set(cacheKey, stock, TimeSpan.FromMinutes(5));
            }

            return stock;
        }

        public async Task<List<decimal>> GetHistoryAsync(string symbol, int days = 7)
        {
            var cacheKey = $"history_{symbol.ToUpper()}_{days}";

            if (_cache.TryGetValue(cacheKey, out List<decimal>? cachedHistory))
            {
                _logger.LogInformation($"Cache hit for history {symbol}");
                return cachedHistory;
            }

            _logger.LogInformation($"Cache miss for history {symbol}, fetching from external API");
            var history = await _yahooService.GetHistoryAsync(symbol, days);

            _cache.Set(cacheKey, history, TimeSpan.FromMinutes(10));
            return history;
        }

        public async Task<StockAnalysis> GetAnalysisAsync(string symbol)
        {
            var cacheKey = $"analysis_{symbol.ToUpper()}";

            if (_cache.TryGetValue(cacheKey, out StockAnalysis? cachedAnalysis))
            {
                _logger.LogInformation($"Cache hit for analysis {symbol}");
                return cachedAnalysis;
            }

            _logger.LogInformation($"Cache miss for analysis {symbol}, calculating");

            var currentStock = await GetStockAsync(symbol);
            var history = await GetHistoryAsync(symbol, 7);

            if (currentStock == null || history.Count == 0)
            {
                throw new InvalidOperationException($"Unable to get data for symbol {symbol}");
            }

            var movingAverage = history.Average();
            var currentPrice = currentStock.Price;
            var trendPercentage = Math.Round(((currentPrice - movingAverage) / movingAverage) * 100, 2);

            string trend = trendPercentage switch
            {
                > 2 => "Up",
                < -2 => "Down",
                _ => "Neutral"
            };

            var analysis = new StockAnalysis
            {
                Symbol = symbol.ToUpper(),
                CurrentPrice = currentPrice,
                MovingAverage7Days = Math.Round(movingAverage, 2),
                Trend = trend,
                TrendPercentage = trendPercentage,
                AnalysisDate = DateTime.Now
            };

            _cache.Set(cacheKey, analysis, TimeSpan.FromMinutes(15));
            return analysis;
        }
    }
}