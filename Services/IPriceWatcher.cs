using System.Threading.Tasks;
using MassTransit;
using CoinTreeViewer.Models;

namespace CoinTreeViewer.Services
{
    public interface IPriceWatcher: IConsumer<LatestPrice>
    {
        CurrencyPrice GetLatestPrice();

        /// <summary>
        /// Starts endless watcher in background thread
        /// </summary>
        void Start();
    }    
}