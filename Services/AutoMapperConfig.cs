using AutoMapper;
using CoinTreeViewer.Database.Models;
using CoinTreeViewer.Models;

namespace CoinTreeViewer.Services
{
    public static class AutoMapperConfig
    {
        public static void Configure(IMapperConfigurationExpression mapper)
        {
            mapper.CreateMap<DbPrice, CurrencyPrice>()
                .ForMember(price => price.BuyPriceDiff, opt => opt.Ignore())
                .ForMember(price => price.SellPriceDiff, opt => opt.Ignore());
        }
    }
}