using FinancialAPI.Models;

namespace FinancialAPI.Services
{
    public class YahooFinanceService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<YahooFinanceService> _logger;

        public YahooFinanceService(HttpClient httpClient, ILogger<YahooFinanceService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<Stock?> GetStockAsync(string symbol)
        {
            try
            {
                return CreateRealisticMockStock(symbol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching data for symbol {symbol}");
                return null;
            }
        }

        private Stock CreateRealisticMockStock(string symbol)
        {
            var random = new Random();
            var companies = new Dictionary<string, string>
            {
                {"AAPL", "Apple Inc."},
                {"MSFT", "Microsoft Corporation"},
                {"GOOGL", "Alphabet Inc."},
                {"AMZN", "Amazon.com Inc."},
                {"TSLA", "Tesla Inc."},
                {"META", "Meta Platforms Inc."},
                {"NVDA", "NVIDIA Corporation"}
            };

            var basePrice = symbol.ToUpper() switch
            {
                "AAPL" => 180m,
                "MSFT" => 340m,
                "GOOGL" => 140m,
                "AMZN" => 150m,
                "TSLA" => 200m,
                "META" => 320m,
                "NVDA" => 800m,
                _ => 100m
            };

            var variation = (decimal)(random.NextDouble() * 20 - 10);
            var currentPrice = basePrice + variation;
            var change = (decimal)(random.NextDouble() * 6 - 3);
            var changePercent = Math.Round((change / currentPrice) * 100, 2);

            return new Stock
            {
                Symbol = symbol.ToUpper(),
                Name = companies.GetValueOrDefault(symbol.ToUpper(), $"{symbol.ToUpper()} Company"),
                Price = Math.Round(currentPrice, 2),
                Change = Math.Round(change, 2),
                ChangePercent = changePercent,
                LastUpdated = DateTime.Now
            };
        }

        public async Task<List<decimal>> GetHistoryAsync(string symbol, int days = 7)
        {
            var random = new Random();
            var currentStock = await GetStockAsync(symbol);
            var history = new List<decimal>();
            var basePrice = currentStock?.Price ?? 100m;

            for (int i = days - 1; i >= 0; i--)
            {
                var variation = (decimal)(random.NextDouble() * 10 - 5);
                var price = Math.Max(1, basePrice + variation);
                history.Add(Math.Round(price, 2));
                basePrice = price;
            }

            return history;
        }
    }
}