namespace CoinTreeViewer.Models
{
    /// <summary>
    /// Same as <see cref="CurrencyPrice"/> but represents MessageBus contract for sending initial DB price to client.
    /// </summary>
    public class LatestPrice : CurrencyPrice
    { }
}