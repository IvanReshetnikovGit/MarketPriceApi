using MarketPriceApi.Data;
using MarketPriceApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.
    AddJsonFile("fintacharts.secrets.json", optional: false, reloadOnChange: true);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<FintachartsService>();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddScoped<AssetSyncService>();
builder.Services.AddSingleton<RealtimePriceService>();


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();
    await tokenService.GetAccessTokenAsync();
}
using (var scope = app.Services.CreateScope())
{
    var priceService = scope.ServiceProvider.GetRequiredService<RealtimePriceService>();
    var fintachartsService = scope.ServiceProvider.GetRequiredService<FintachartsService>();
    var allAssets = await fintachartsService.GetSupportedAssetsAsync();
    var allSymbols = allAssets.Select(a => a.Symbol).ToArray();
    await priceService.StartAsync(allSymbols);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

await app.RunAsync();