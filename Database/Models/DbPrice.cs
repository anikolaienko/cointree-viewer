using System;

namespace CoinTreeViewer.Database.Models
{
    public class DbPrice
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        public decimal BuyPrice { get; set; }

        public decimal SellPrice { get; set; }

        public DateTime Timestamp { get; set; }
    }
}