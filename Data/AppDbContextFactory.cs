using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MarketPriceApi.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("fintacharts.secrets.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = config.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            
            optionsBuilder.UseMySQL(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
