using System.Collections.Concurrent;
using System.Text.Json;
using MarketPriceApi.Interfaces;
using MarketPriceApi.Models;
using Websocket.Client;
namespace MarketPriceApi.Services;
public class RealtimePriceService : IRealtimePriceService
{
    private readonly TokenService _tokenService;
    private readonly FintachartsService _fintacharts;
    private readonly ILogger<RealtimePriceService> _logger;
    private WebsocketClient? _client;
    private readonly ConcurrentDictionary<string, AssetPrice> _prices = new();

    public RealtimePriceService(TokenService tokenService, FintachartsService fintacharts, ILogger<RealtimePriceService> logger)
    {
        _tokenService = tokenService;
        _fintacharts = fintacharts;
        _logger = logger;
    }

    public async Task StartAsync(IEnumerable<string> symbolsToSubscribe)
    {
        var token = await _tokenService.GetAccessTokenAsync();
        var url = new Uri($"wss://platform.fintacharts.com/api/streaming/ws/v1/realtime?token={token}");

        _client = new WebsocketClient(url);
        _client.ReconnectTimeout = TimeSpan.FromSeconds(30);

        _client.MessageReceived.Subscribe(msg =>
        {
            if (msg.Text is null) return;

            try
            {
                var doc = JsonDocument.Parse(msg.Text);
                var root = doc.RootElement;

                if (root.GetProperty("type").GetString() == "l1-update")
                {
                    var instrumentId = root.GetProperty("instrumentId").GetString()!;
                    var price = root.GetProperty("last").GetProperty("price").GetDecimal();

                    var symbol = _fintacharts.ResolveSymbolByInstrumentId(instrumentId);
                    if (symbol != null)
                    {
                        _prices[symbol] = new AssetPrice
                        {
                            Symbol = symbol,
                            Price = price,
                            LastUpdated = DateTime.UtcNow
                        };
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to parse");
            }
        });

        await _client.Start();

        var allAssets = await _fintacharts.GetSupportedAssetsAsync();
        var symbolToId = allAssets
            .Where(a => symbolsToSubscribe.Contains(a.Symbol))
            .ToDictionary(a => a.Symbol, a => a.Id);

        _fintacharts.CacheInstrumentMap(symbolToId);

        int id = 1;
        foreach (var (symbol, instrumentId) in symbolToId)
        {
            var payload = new
            {
                type = "l1-subscription",
                id = id++.ToString(),
                instrumentId,
                provider = "oanda",
                subscribe = true,
                kinds = new[] { "last" }
            };

            var json = JsonSerializer.Serialize(payload);
            _client.Send(json);
        }
    }

    public List<AssetPrice> GetPrices(IEnumerable<string> symbols)
    {
        return symbols
            .Where(_prices.ContainsKey)
            .Select(symbol => _prices[symbol])
            .ToList();
    }
}