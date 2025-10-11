using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace XgpLib.SyncService.Infrastructure.Data;

public class XgpLibDbContextFactory : IDesignTimeDbContextFactory<XgpLibDbContext>
{
    public XgpLibDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<XgpLibDbContext>();
        optionsBuilder
            .UseNpgsql(configuration.GetConnectionString("XgpLibPostgresConnection"))
            .UseSnakeCaseNamingConvention();

        return new XgpLibDbContext(optionsBuilder.Options);
    }
}
