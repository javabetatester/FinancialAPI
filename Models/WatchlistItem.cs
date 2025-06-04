using System.ComponentModel.DataAnnotations;

namespace FinancialAPI.Models
{
    public class WatchlistItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string Symbol { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public DateTime AddedAt { get; set; }
    }
}