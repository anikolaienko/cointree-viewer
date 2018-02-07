using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using AutoMapper;
using CoinTreeViewer.Database;
using CoinTreeViewer.Database.Models;
using CoinTreeViewer.Models;

namespace CoinTreeViewer.Services
{
    public class DbPriceConsumer1 : IConsumer<CurrencyPrice>
    {
        public async Task Consume(ConsumeContext<CurrencyPrice> context)
        {
        }
    }

    public class DbPriceConsumer : IConsumer<CurrencyPrice>
    {
        private readonly IBus _messageBus;
        private readonly IMapper _mapper;

        public DbPriceConsumer(IBus messageBus,
                            IMapper mapper)
        {
            _messageBus = messageBus;
            _mapper = mapper;
            
            // Probably not the best approach to do that in ctor,
            // but it publishes very first price from DB to message bus on app start.
            using (var dbContext = new AppDbContext())
            {
                var price = dbContext.GetLatestPrice();
                if (price != null)
                {
                    _messageBus.Publish(_mapper.Map<CurrencyPrice>(price));
                }
            }
        }

        public async Task Consume(ConsumeContext<CurrencyPrice> context)
        {
            using (var dbContext = new AppDbContext())
            {
                if (context.Message != null)
                {
                    await dbContext.AddAsync(_mapper.Map<DbPrice>(context.Message));
                }
            }
        }
    }
}