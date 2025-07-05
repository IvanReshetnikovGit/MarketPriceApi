using MarketPriceApi.Models;
namespace MarketPriceApi.Interfaces;

public interface IRealtimePriceService
{
    public Task StartAsync(IEnumerable<string> symbolsToSubscribe);
    public List<AssetPrice> GetPrices(IEnumerable<string> symbols);
}