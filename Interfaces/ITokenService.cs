using MarketPriceApi.Models;
namespace MarketPriceApi.Interfaces;

public interface ITokenService
{
    Task<string> GetAccessTokenAsync();
}