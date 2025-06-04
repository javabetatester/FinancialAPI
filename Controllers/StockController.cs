using Microsoft.AspNetCore.Mvc;
using FinancialAPI.Models;

namespace FinancialAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StocksController : ControllerBase
    {
        [HttpGet("{symbol}")]
        public ActionResult<Stock> GetStock(string symbol)
        {
            // Dados mockados simples para terminar a Hora 1
            var stock = new Stock
            {
                Symbol = symbol.ToUpper(),
                Name = $"{symbol.ToUpper()} Company",
                Price = 150.00m,
                Change = 2.50m,
                ChangePercent = 1.69m,
                LastUpdated = DateTime.Now
            };

            return Ok(stock);
        }
    }
}