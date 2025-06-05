using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinancialAPI.Models;
using FinancialAPI.Data;
using FinancialAPI.Services;

namespace FinancialAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WatchlistController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IStockService _stockService;
        private readonly ILogger<WatchlistController> _logger;

        public WatchlistController(AppDbContext context, IStockService stockService, ILogger<WatchlistController> logger)
        {
            _context = context;
            _stockService = stockService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<object>>> GetWatchlist()
        {
            try
            {
                var watchlistItems = await _context.WatchlistItems
                    .OrderBy(w => w.Symbol)
                    .ToListAsync();

                var enrichedWatchlist = new List<object>();

                foreach (var item in watchlistItems)
                {
                    try
                    {
                        var stock = await _stockService.GetStockAsync(item.Symbol);
                        enrichedWatchlist.Add(new
                        {
                            Id = item.Id,
                            Symbol = item.Symbol,
                            Name = item.Name,
                            AddedAt = item.AddedAt,
                            CurrentPrice = stock?.Price,
                            Change = stock?.Change,
                            ChangePercent = stock?.ChangePercent,
                            LastUpdated = stock?.LastUpdated
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Error fetching stock data for {item.Symbol}");
                        enrichedWatchlist.Add(new
                        {
                            Id = item.Id,
                            Symbol = item.Symbol,
                            Name = item.Name,
                            AddedAt = item.AddedAt,
                            CurrentPrice = (decimal?)null,
                            Change = (decimal?)null,
                            ChangePercent = (decimal?)null,
                            LastUpdated = (DateTime?)null,
                            Error = "Failed to fetch current data"
                        });
                    }
                }

                return Ok(enrichedWatchlist);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching watchlist");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<WatchlistItem>> AddToWatchlist(WatchlistItem item)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(item.Symbol))
                {
                    return BadRequest("Symbol is required");
                }

                if (string.IsNullOrWhiteSpace(item.Name))
                {
                    return BadRequest("Name is required");
                }

                // Verificar se já existe
                var existingItem = await _context.WatchlistItems
                    .FirstOrDefaultAsync(w => w.Symbol.ToUpper() == item.Symbol.ToUpper());

                if (existingItem != null)
                {
                    return Conflict($"Symbol {item.Symbol} already exists in watchlist");
                }

                // Verificar se o símbolo é válido tentando buscar dados
                var stock = await _stockService.GetStockAsync(item.Symbol);
                if (stock == null)
                {
                    return BadRequest($"Invalid symbol: {item.Symbol}");
                }

                item.Symbol = item.Symbol.ToUpper();
                item.AddedAt = DateTime.Now;

                _context.WatchlistItems.Add(item);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetWatchlist), new { id = item.Id }, item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to watchlist");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromWatchlist(int id)
        {
            try
            {
                var item = await _context.WatchlistItems.FindAsync(id);
                if (item == null)
                {
                    return NotFound($"Watchlist item with id {id} not found");
                }

                _context.WatchlistItems.Remove(item);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing item {id} from watchlist");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}