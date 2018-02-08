using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using CoinTreeViewer.Database.Models;

namespace CoinTreeViewer.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<DbPrice> Prices { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}