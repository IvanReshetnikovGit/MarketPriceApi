namespace MarketPriceApi.Models;
public class AssetPrice
{
    public required string Symbol { get; set; }
    public decimal Price { get; set; }
    public DateTime LastUpdated { get; set; }
}
