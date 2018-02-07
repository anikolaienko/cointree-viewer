using System;
using Microsoft.AspNetCore.SignalR;

using CoinTreeViewer.Models;

namespace CoinTreeViewer.Services
{
    public class PriceWatcherHub : Hub
    {
        private readonly IPriceWatcher _priceWatcher;

        public PriceWatcherHub(IPriceWatcher priceWatcher)
        {
            _priceWatcher = priceWatcher;
        }

        public CurrencyPrice GetLatestPrice()
        {
            return _priceWatcher.GetLatestPrice();
        }
    }
}