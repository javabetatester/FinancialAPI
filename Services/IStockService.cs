using FinancialAPI.Models;

namespace FinancialAPI.Services
{
    public interface IStockService
    {
        Task<Stock?> GetStockAsync(string symbol);
        Task<List<decimal>> GetHistoryAsync(string symbol, int days = 7);
        Task<StockAnalysis> GetAnalysisAsync(string symbol);
    }
}