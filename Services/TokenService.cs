using System.Text.Json;
using MarketPriceApi.Interfaces;
namespace MarketPriceApi.Services;
public class TokenService : ITokenService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<TokenService> _logger;

    private string? _accessToken;
    private DateTime _accessTokenExpiresAt;

    public TokenService(HttpClient httpClient, IConfiguration config, ILogger<TokenService> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        if (_accessToken == null)
        {
            _logger.LogInformation("🔑 Access token unavailible");
            await LoginAsync();
        }
        else if (DateTime.UtcNow >= _accessTokenExpiresAt)
        {
            _logger.LogInformation("🔑 Access token outdated");
            await LoginAsync();
        }
        else
        {
            _logger.LogInformation("🔑 Access token valid");
        }

        return _accessToken!;
    }


    private async Task LoginAsync()
    {
        var url = $"{_config["Fintacharts:BaseUrl"]}/identity/realms/{_config["Fintacharts:Realm"]}/protocol/openid-connect/token";

        var parameters = new Dictionary<string, string>
        {
            { "grant_type", "password" },
            { "client_id", _config["Fintacharts:ClientId"]! },
            { "username", _config["Fintacharts:Username"]! },
            { "password", _config["Fintacharts:Password"]! }
        };

        var response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(parameters));
        response.EnsureSuccessStatusCode();

        await ParseTokenResponseAsync(response);

        _logger.LogInformation("✅ Access token obtained successfully.");
    }

    private async Task ParseTokenResponseAsync(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        _accessToken = root.GetProperty("access_token").GetString();
        var expiresIn = root.GetProperty("expires_in").GetInt32();

        _accessTokenExpiresAt = DateTime.UtcNow.AddSeconds(expiresIn - 60);
    }
}
