using Microsoft.EntityFrameworkCore;
using FinancialAPI.Models;

namespace FinancialAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<WatchlistItem> WatchlistItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WatchlistItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Symbol).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Symbol);
            });
        }
    }
}