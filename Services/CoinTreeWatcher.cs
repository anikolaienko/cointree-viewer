using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using MassTransit;
using CoinTreeViewer.Models;


namespace CoinTreeViewer.Services
{
    public class CoinTreeWatcher : IPriceWatcher
    {
        private const string BTCPriceUrl = "https://api.cointree.com.au/v1/price/btc/aud";
        private const string CurrencyName = "Bitcoin";
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(5);
        private readonly IHubContext<PriceWatcherHub> _clients;
        private readonly ILogger _logger;
        private readonly IBus _messageBus;
        private Timer _timer;
        private CurrencyPrice _latestPrice = null;

        public CoinTreeWatcher(IHubContext<PriceWatcherHub> clients,
                            IBus messageBus,
                            ILoggerFactory loggerFactory) 
        {
            _logger = loggerFactory.CreateLogger<CoinTreeWatcher>();
            _messageBus = messageBus;
            _clients = clients;
        }

        /// <summary>
        /// Starts endless watcher in background thread
        /// </summary>
        public void Start(CurrencyPrice latestPrice = null)
        {
            _latestPrice = latestPrice;
            
            if (latestPrice == null)
            {
                // for very first launch when there is no initial value from DB.
                FillPrice(null);
            }

            if (_timer != null)
            {
                _timer.Dispose();
            }
            _timer = new Timer(FillPrice, null, _updateInterval, _updateInterval);
        }

        public CurrencyPrice GetLatestPrice()
        {
            return _latestPrice;
        }

        public void FillPrice(object state)
        {
            var client = new HttpClient();
            var response = client.GetAsync(BTCPriceUrl).Result;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = response.Content.ReadAsStringAsync();
                var jsonData = JObject.Parse(content.Result);
                var buyPrice = jsonData.GetValue("ask").Value<decimal>();
                var sellPrice = jsonData.GetValue("bid").Value<decimal>();

                var price = new CurrencyPrice()
                {
                    Name = CurrencyName,
                    BuyPrice = buyPrice,
                    BuyPriceDiff = _latestPrice != null ? GetDiff(_latestPrice.BuyPrice, buyPrice) : 0,
                    SellPrice = sellPrice,
                    SellPriceDiff = _latestPrice != null ? GetDiff(_latestPrice.SellPrice, sellPrice) : 0,
                    Timestamp = DateTime.UtcNow
                };

                _messageBus.Publish(price);
                Task.Factory.StartNew(() => BroadcastPriceAsync(price));
            }
            else
            {
                _logger.LogError(3, $"CoinTree response status: {response.StatusCode}; message: {response.Content.ReadAsStringAsync().Result}");
            }
        }

        private async Task BroadcastPriceAsync(CurrencyPrice price)
        {
            await _clients.Clients.All.InvokeAsync("updateCurrencyPrice", price);
        }

        public async Task Consume(ConsumeContext<LatestPrice> context)
        {
            if (context.Message != null && _latestPrice == null)
            {
                await BroadcastPriceAsync(context.Message);
            }
        }

        private decimal GetDiff(decimal oldPrice, decimal newPrice)
        {
            return (newPrice - oldPrice) / oldPrice;
        }
    }
}