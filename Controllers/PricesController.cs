using MarketPriceApi.Models;
using MarketPriceApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarketPriceApi.Controllers;

[ApiController]
[Route("api")]
public class PricesController : ControllerBase
{
    private readonly RealtimePriceService _priceService;

    public PricesController(RealtimePriceService priceService)
    {
        _priceService = priceService;
    }
    [HttpGet("prices")]
    [ProducesResponseType(typeof(AssetPrice), 200)]
    public IActionResult GetPrices([FromQuery] string symbols)
    {
        if (string.IsNullOrWhiteSpace(symbols))
            return BadRequest("Please provide symbols");

        var symbolList = symbols.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var prices = _priceService.GetPrices(symbolList);

        if (!prices.Any())
            return NotFound("No prices found");

        return Ok(prices);
    }

}