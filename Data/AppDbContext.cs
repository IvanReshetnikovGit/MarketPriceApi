using Microsoft.EntityFrameworkCore;
using MarketPriceApi.Models;

namespace MarketPriceApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Asset> Assets => Set<Asset>();
}
