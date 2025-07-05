using System.Net.Http.Headers;
using System.Text.Json;
using MarketPriceApi.Interfaces;
using MarketPriceApi.Models;

namespace MarketPriceApi.Services;

public class FintachartsService : IFintachartsService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly TokenService _tokenService;
    private readonly ILogger<FintachartsService> _logger;
    private Dictionary<string, string> _instrumentIdToSymbol = new();

    public FintachartsService(HttpClient httpClient, IConfiguration config, TokenService tokenService, ILogger<FintachartsService> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<List<Asset>> GetSupportedAssetsAsync(string provider = "oanda", string kind = "forex")
    {
        string apiKey = await _tokenService.GetAccessTokenAsync();
        string baseUrl = _config["Fintacharts:BaseUrl"]!;

        var url = $"{baseUrl}/api/instruments/v1/instruments?provider={provider}&kind={kind}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var doc = JsonDocument.Parse(json);

        var assets = new List<Asset>();

        if (doc.RootElement.TryGetProperty("data", out var dataElement) && dataElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in dataElement.EnumerateArray())
            {
                var id = item.GetProperty("id").GetString() ?? "";
                var symbol = item.GetProperty("symbol").GetString() ?? "";

                assets.Add(new Asset
                {
                    Id = id,
                    Symbol = symbol,
                });
            }
        }
        else
        {
            _logger.LogWarning("Unexpected JSON structure: {Json}", json);
        }
        return assets;
    }
    public void CacheInstrumentMap(Dictionary<string, string> symbolToId)
    {
        _instrumentIdToSymbol = symbolToId.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
    }

    public string? ResolveSymbolByInstrumentId(string instrumentId)
    {
        return _instrumentIdToSymbol.TryGetValue(instrumentId, out var symbol) ? symbol : null;
    }
}
