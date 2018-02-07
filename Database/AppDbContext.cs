using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using CoinTreeViewer.Database.Models;

namespace CoinTreeViewer.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<DbPrice> Prices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=prices.db");
        }

        public DbPrice GetLatestPrice()
        {
            return new DbPrice();
            return Prices.OrderByDescending(price => price.Timestamp).FirstOrDefault();
        }
    }
}