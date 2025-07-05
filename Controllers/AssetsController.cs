using MarketPriceApi.Models;
using MarketPriceApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarketPriceApi.Controllers;
[ApiController]
[Route("api")]
public class AssetsController : ControllerBase
{
    private readonly AssetSyncService _assetSyncService;

    public AssetsController(
        AssetSyncService assetSyncService)
    {
        _assetSyncService = assetSyncService;
    }

    [HttpGet("assets")]
    [ProducesResponseType(typeof(Asset), 200)]
    public async Task<IActionResult> GetAllAssets()
    {
        var result = await _assetSyncService.SyncAndGetAssetsAsync();
        return Ok(new { SupportedAssets = result });
    }
}
