using MarketPriceApi.Models;

namespace MarketPriceApi.Interfaces;

public interface IFintachartsService
{
    Task<List<Asset>> GetSupportedAssetsAsync(string provider = "oanda", string kind = "forex");
    public void CacheInstrumentMap(Dictionary<string, string> symbolToId);
    public string? ResolveSymbolByInstrumentId(string instrumentId);
}