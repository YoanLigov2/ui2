using Microsoft.EntityFrameworkCore;
using WebApplication11.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication11.Data
{
    public class StockContext : DbContext
    {
        public StockContext(DbContextOptions<StockContext> options) : base(options) { }

        public DbSet<StockPrice> StockPrices { get; set; }
    }
}