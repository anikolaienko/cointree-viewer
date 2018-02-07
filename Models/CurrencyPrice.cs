using System;

namespace CoinTreeViewer.Models
{
    public class CurrencyPrice
    {
        public string Name { get; set; }

        public decimal BuyPrice { get; set; }

        public decimal BuyPriceDiff { get; set; }

        public decimal SellPrice { get; set; }

        public decimal SellPriceDiff { get; set; }

        public DateTime Timestamp { get; set; }
    }
}