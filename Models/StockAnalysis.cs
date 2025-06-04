namespace FinancialAPI.Models
{
    public class StockAnalysis
    {
        public string Symbol { get; set; } = string.Empty;
        public decimal CurrentPrice { get; set; }
        public decimal MovingAverage7Days { get; set; }
        public string Trend { get; set; } = string.Empty; // "Up", "Down", "Neutral"
        public decimal TrendPercentage { get; set; }
        public DateTime AnalysisDate { get; set; }
    }
}