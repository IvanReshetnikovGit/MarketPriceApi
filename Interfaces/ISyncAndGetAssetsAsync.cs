using MarketPriceApi.Models;

namespace MarketPriceApi.Interfaces;

public interface ISyncAndGetAssetsAsync
{
    public Task<List<Asset>> SyncAndGetAssetsAsync();
    
}