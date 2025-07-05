using MarketPriceApi.Models;
using MarketPriceApi.Data;
using Microsoft.EntityFrameworkCore;
using MarketPriceApi.Interfaces;
namespace MarketPriceApi.Services;
public class AssetSyncService : ISyncAndGetAssetsAsync
{
    private readonly AppDbContext _context;
    private readonly FintachartsService _fintacharts;

    public AssetSyncService(AppDbContext context, FintachartsService fintacharts)
    {
        _context = context;
        _fintacharts = fintacharts;
    }

    public async Task<List<Asset>> SyncAndGetAssetsAsync()
    {
        var remoteAssets = await _fintacharts.GetSupportedAssetsAsync();

        if (remoteAssets == null || !remoteAssets.Any())
        {
            return await _context.Assets.ToListAsync();
        }

        var localAssets = await _context.Assets.ToListAsync();
        var updated = false;

        var localAssetsDict = localAssets.ToDictionary(a => a.Id);

        foreach (var remote in remoteAssets)
        {
            if (localAssetsDict.TryGetValue(remote.Id, out var local))
            {
                if (local.Symbol != remote.Symbol)
                {
                    local.Symbol = remote.Symbol;
                    updated = true;
                }
            }
            else
            {
                _context.Assets.Add(remote);
                updated = true;
            }
        }

        if (updated)
        {
            await _context.SaveChangesAsync();
        }

        return await _context.Assets.ToListAsync();
    }
}
