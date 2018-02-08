using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using MassTransit;
using AutoMapper;
using CoinTreeViewer.Database;
using CoinTreeViewer.Database.Models;
using CoinTreeViewer.Models;
using Microsoft.Extensions.Logging;

namespace CoinTreeViewer.Services
{
    public class DbPriceConsumer : IConsumer<CurrencyPrice>
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;

        public DbPriceConsumer(AppDbContext dbContext,
                            ILoggerFactory loggingFactory,
                            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = loggingFactory.CreateLogger<DbPriceConsumer>();
        }

        public CurrencyPrice GetLatestPrice()
        {
            DbPrice dbPrice = null;
            try
            {
                dbPrice = _dbContext.Prices
                    .OrderByDescending(price => price.Timestamp)
                    .FirstOrDefault();
            }
            catch(SqliteException ex)
            {
                // in case bd not created yet.
                _logger.LogError(ex, "Can be expeceted cause db is completely emtpy");
            }

            return _mapper.Map<CurrencyPrice>(dbPrice);
        }

        public async Task Consume(ConsumeContext<CurrencyPrice> context)
        {
            if (context.Message != null)
            {
                await _dbContext.Prices.AddAsync(_mapper.Map<DbPrice>(context.Message));
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}